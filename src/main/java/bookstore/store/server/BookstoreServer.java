package bookstore.store.server;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.rmi.RemoteException;
import java.util.LinkedList;

import bookstore.store.commons.BaseRMI;
import bookstore.store.server.responses.Request;
import bookstore.store.server.responses.Book;

public class BookstoreServer extends BaseRMI implements ServerInterface {
    private static final int OBJ_PORT = 8005;
    private static final String OBJ_NAME = "BookstoreServer";
    private static final String DB_URL = "jdbc:mysql://localhost:3306/bookstore?useJDBCCompliantTimezoneShift=true&useLegacyDatetimeCode=false&serverTimezone=UTC";
    private static final String USERNAME = "root";
    private static final String PASSWORD = "Pass123!";

    private static final String USER_REQUESTS_QUERY = "SELECT Request.book_title AS title, Request.amount, Request.disp_date AS date, Request.req_state"
            + " FROM Request, SingleOrder"
            + " WHERE SingleOrder.client_name = ? AND SingleOrder.id = Request.order_id;";
    private static final String ALL_BOOKS_QUERY = "SELECT * FROM Book;";

    public static void main(String[] args) {
        System.out.println("Starting Bookstore server...");
        try {
            ServerInterface server = new BookstoreServer();
            ServerInterface stub = (ServerInterface)registerObject(server, OBJ_NAME, OBJ_PORT);
            if (stub != null) {
                System.out.println("Bookstore server is up!");
                System.in.read();
            }
        } catch (Exception e) {
            System.err.println("Bookstore server exception:");
            e.printStackTrace();
        }
    }

    private Connection getConnection() {
        try {
            Connection conn = DriverManager.getConnection(DB_URL, USERNAME, PASSWORD);
            conn.setAutoCommit(false);
            return conn;
        } 
        catch (SQLException e) {
            System.err.println(e);
            return null;
        }
    }

    @Override
    public Request getUserRequests(String username) throws RemoteException, SQLException {
        System.out.println("Getting requests for user '" + username + "'");
        Connection conn = this.getConnection();
        if (conn != null) {
            PreparedStatement pstmt = conn.prepareStatement(USER_REQUESTS_QUERY);
            pstmt.setNString(1, username);
            ResultSet res = pstmt.executeQuery();
            Request req = new Request(res);

            conn.commit();
            pstmt.close();
            return req;
        }
        return null;
    }

    @Override
    public LinkedList<Book> getAllBooks() throws RemoteException, SQLException {
        System.out.println("Getting all store books");
        Connection conn = this.getConnection();
        if (conn != null) {
            PreparedStatement pstmt = conn.prepareStatement(ALL_BOOKS_QUERY);
            ResultSet res = pstmt.executeQuery();
            conn.commit();

            LinkedList<Book> books = new LinkedList<>();
            while (res.next()) {
                books.add(new Book(res.getString("title"), res.getDouble("price"), res.getInt("stock")));
            }
            pstmt.close();
            return books;
        }

        return null;
    }

    @Override
    public String putRequest(Request new_request) throws RemoteException, SQLException {
        return null;
    }

}