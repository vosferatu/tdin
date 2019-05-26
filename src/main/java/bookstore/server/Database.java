package bookstore.server;

import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.Map;
import java.util.concurrent.ConcurrentHashMap;
import java.util.stream.Stream;

import bookstore.server.responses.Book;
import bookstore.server.responses.BookOrder;
import bookstore.server.responses.BookRequests;
import bookstore.server.responses.Request;

class Database {
    //Used in both servers
    private ConcurrentHashMap<String, Book> books;
    //Used only in bookstore server
    private ConcurrentHashMap<String, Integer> book_stock;
    // Used only in bookstore server
    private ConcurrentHashMap<String, LinkedList<Long>> arrived_books;
    //USed in both servers
    private LinkedList<Request> requests;


    private boolean is_bookstore;

    Database(boolean is_bookstore) {
        this.is_bookstore = is_bookstore;
        this.books = new ConcurrentHashMap<>();
        this.book_stock = new ConcurrentHashMap<>();
        this.readBooksFile();
        this.arrived_books = new ConcurrentHashMap<>();
        if (this.is_bookstore) {
            this.readStockFile();
        }
        this.requests = new LinkedList<>();
    }

    private void readBooksFile() {
        try (Stream<String> stream = Files.lines(Paths.get("assets/database/books.csv"))) {
            stream.forEach((String line) -> {
                String[] parts = line.split(";");
                if (parts.length == 2) {
                    this.books.put(parts[0], new Book(parts[0], Double.parseDouble(parts[1]), Integer.MIN_VALUE));
                    this.book_stock.put(parts[0], 0);
                }
                else throw new RuntimeException("books.csv is malformed on line:\n " + line);
            });
        }
        catch (Exception e) {
            System.err.println("Failed to process books.csv!\n - " + e);
            e.printStackTrace();
        }
    }

    private void readStockFile() {
        try (Stream<String> stream = Files.lines(Paths.get("assets/database/stock.csv"))) {
            stream.forEach((String line) -> {
                String[] parts = line.split(";");
                if (parts.length == 2) {
                    if (this.books.containsKey(parts[0])) {
                        this.book_stock.compute(parts[0], (String t, Integer a) -> Integer.parseInt(parts[1]));
                    }
                    else throw new RuntimeException("Book title '" + parts[0] + "' does not exist in books.csv!");
                    
                } else
                    throw new RuntimeException("stock.csv is malformed on line:\n " + line);
            });
        } catch (Exception e) {
            System.err.println("Failed to process stock.csv!\n - " + e);
        }
    }

    LinkedList<Request> getUserRequests(String username) {
        System.out.println("Getting requests for user '" + username + "'");
        LinkedList<Request> requests = new LinkedList<>();
        synchronized (this.requests) {
            for (Request req : this.requests) {
                if (req.getClientName().equals(username)) {
                    requests.add(req);
                }
            }
        }
        return requests;
    }

    LinkedList<Request> getWaitingRequests() {
        LinkedList<Request> reqs = new LinkedList<Request>();
        for (Request req : requests) {
            if (req.isWaiting()) {
                reqs.add(req);
            }
        }

        return reqs;
    }

    LinkedList<Book> getAllBooks() {
        System.out.println("Getting all store books");
        LinkedList<Book> books = new LinkedList<>();
        synchronized (this.books) {
            this.books.forEach((String title, Book book) -> {
                book.setStock(this.book_stock.get(title));
                books.add(book);
            });
        }

        return books;
    }

    Map<String, Book> getBooks() {
        return this.books;
    }

    /**
     * Function called by the warehouse client to both the warehouse server and bookstore server
     * @param title Title of book dispatched
     * @param ids   ID's of the orders that had that book
     */
    void bookDispatched(String title, LinkedList<Long> ids) {
        synchronized (this.arrived_books) {
            synchronized (this.requests) {
                for (Request req : this.requests) {
                    if (req.hasWaitingBook(title) && ids.contains(req.getID())) {
                        req.setBookDispatching(title);
                    }
                }
            }
    
            if (this.arrived_books.containsKey(title)) {
                LinkedList<Long> req_uuids = this.arrived_books.get(title);
                synchronized (req_uuids) {
                    for (Long id : ids) {
                        if (!req_uuids.contains(id)) req_uuids.add(id);
                    }
                }
            }
            else {
                this.arrived_books.put(title, ids);
            }
        }
    }

    LinkedList<BookRequests> getArrivedBooks() {
        LinkedList<BookRequests> books_requests = new LinkedList<>();
        synchronized (this.arrived_books) {
            synchronized (this.requests) {
                this.arrived_books.forEach((String title, LinkedList<Long> ids) -> {
                    int amount = 10;
                    synchronized (ids) {
                        for (Request req : this.requests) {
                            if (ids.contains(req.getID()) && req.hasDispatchingBook(title)) {
                                amount += req.getBookAmount(title);
                            }
                        }
                    }
                    if (amount > 10) {
                        books_requests.add(new BookRequests(title, amount, ids));
                    }
                });
            }
        }
        return books_requests;
    }

    void booksStored(LinkedList<BookRequests> books_req) {
        synchronized (this.arrived_books) {
            synchronized (this.requests) {
                for (Request req : this.requests) {
                    long id = req.getID();
                    HashMap<String, Integer> dispatched_books = new HashMap<>();
                    for (BookRequests book_req : books_req) {
                        String title = book_req.getTitle();
                        LinkedList<Long> req_ids = book_req.getReqsID();

                        if (req_ids.contains(id) && req.hasDispatchingBook(title)) {
                            req.setBookDispatched(title);
                            dispatched_books.put(title, req.getBookAmount(title));
                            if (this.arrived_books.containsKey(title)) {
                                this.arrived_books.get(title).remove(id);
                            }
                        }
                        this.book_stock.compute(title, (String t, Integer a) -> (a == null ? 0 : a) + 10);
                    }
                    this.sendBooksDispatchedEmail(req.getClientName(), req.getEmail(), req.getAddress(), req.getID(), dispatched_books);
                }
            }
        }
        FileWriter.writeStocksToFile(this.book_stock);
    }

    private void sendBooksDispatchedEmail(String name, String email, String addr, long id, HashMap<String, Integer> dispatched_books) {
        String txt = "";
        for (Map.Entry<String, Integer> entry : dispatched_books.entrySet()) {
            String title = entry.getKey();
            int amount = entry.getValue();

            txt += String.format(" -> %d " + (amount > 1 ? "copies" : "copy") 
                + " of '%s'\n", amount, title);
        }
        String body = String.format("Dear Mr. or Mrs. %s\n\n" 
            + "We are pleased to inform you that the previously requested books:\n"
            + txt + "\nAre being dispatched today so they should arrive briefly at your home address '%s'"
            + "\n\nBest Regards,\nJoao Almeida\nJoao Mendes", name, addr);
        EmailDispatcher.sendEmail(email, String.format("Dispatched Order #%d Books", id), body);
    }

    /**
     * Stores the request in the data structures
     * It also sorts which book orders can already be completed and which ones cannot
     * @param new_req   New Request to be added
     * @return HashMap with 2 keys ["finished", "unfinished"], each pointing to a list of book orders.
     * Unfinished orders need to be sent to the warehouse and finished need to send email to the user
     */
    private HashMap<String, Request> putRequestBookstore(Request new_req) {
        HashMap<String, LinkedList<BookOrder>> orders = new HashMap<>(2);
        orders.put("finished", new LinkedList<BookOrder>());
        orders.put("unfinished", new LinkedList<BookOrder>());

        synchronized (this.book_stock) {
            for (BookOrder order : new_req.getRequestBooks()) {
                String title = order.getTitle();
                if (this.book_stock.containsKey(title) && this.book_stock.get(title) >= order.getAmount()) {
                    order.setDispatchedAt(false, 1);
                    this.book_stock.computeIfPresent(title, 
                        (String book_title, Integer amount) -> amount - order.getAmount());                     
                    orders.get("finished").add(order);
                    continue;
                }
                order.setAwaitingState();
                orders.get("unfinished").add(order);
            }
        }
        FileWriter.writeStocksToFile(this.book_stock);
        synchronized (this.requests) { this.requests.add(new_req); }
        return this.ordersToRequest(new_req, orders);
    }

    public HashMap<String, Request> putRequest(Request new_req) {
        if (this.is_bookstore) {
            return this.putRequestBookstore(new_req);
        }
        else {
            synchronized (this.requests) {
                this.requests.add(new_req);
            }
            return null;
        }
    }

    private HashMap<String, Request> ordersToRequest(Request new_req, HashMap<String, LinkedList<BookOrder>> orders) {
        HashMap<String, Request> requests = new HashMap<>(2);
        requests.put("finished", Request.fromOtherRequest(new_req, orders.get("finished")));
        requests.put("unfinished", Request.fromOtherRequest(new_req, orders.get("unfinished")));
    
        return requests;
    }
}