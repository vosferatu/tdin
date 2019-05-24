package bookstore.webapp.controllers;

import javax.servlet.http.*;

import bookstore.commons.BaseRMI;
import bookstore.server.responses.Book;
import bookstore.server.ServerInterface;
import bookstore.server.responses.Request;
import bookstore.server.responses.BookOrder;

import java.io.IOException;
import java.util.ArrayList;
import java.util.LinkedList;
import java.util.List;

import javax.servlet.ServletException;
import javax.servlet.annotation.WebServlet;

@WebServlet(name = "ClientController", urlPatterns = "/login")
public class ClientController extends HttpServlet {
    private static final long serialVersionUID = -7012574937926535948L;
    private static final String OBJ_NAME = "BookstoreServer";
    private static final int OBJ_PORT = 8005;

    private ServerInterface server_obj;

    private static class Client {
        private final String username;
        private final String email;

        private Client(String user, String email) {
            this.username = user;
            this.email = email;
        }

        public static Client fromReqAttributes(HttpServletRequest req) {
            return new Client(req.getParameter("username"), req.getParameter("email"));
        }

        public void setAsReqAttributes(HttpServletRequest req) {
            req.setAttribute("username", this.username);
            req.setAttribute("email", this.email);
        }

        public List<String> validate() {
            List<String> errors = new ArrayList<>();
            if (this.username.length() == 0) {
                errors.add("Username not valid!");
            }
            if (this.email.length() == 0) { // TODO: add better validation
                errors.add("Email not valid!");
            }
            return errors;
        }
    }

    public ClientController() {
        this.server_obj = (ServerInterface)BaseRMI.fetchObject(OBJ_NAME, OBJ_PORT);
    }

    @Override
    protected void doPost(HttpServletRequest req, HttpServletResponse res)
            throws ServletException, IOException
    {
        Client client = Client.fromReqAttributes(req);
        client.setAsReqAttributes(req);
        List<String> violations = client.validate();
        String url = "/WEB-INF/views/homepage.jsp";

        if (!violations.isEmpty()) {
            req.setAttribute("violations", violations);
            url = "/";
        }
        else {
            req.setAttribute("requested_books", this.getUserBooks(req.getParameter("username")));
            req.setAttribute("all_books", this.getAllBooks());
        }

        req.getRequestDispatcher(url).forward(req, res);
    }

    private List<BookOrder> getUserBooks(String username) {
        try {
            LinkedList<Request> reqs = this.server_obj.getUserRequests(username);
            LinkedList<BookOrder> books = new LinkedList<>();
            for (Request req : reqs) {
                books.addAll(req.getRequestBooks());
            }
            return books;
        } catch (Exception e) {
            System.err.println("Error!\n - " + e);
        }

        return null;
    }

    private LinkedList<Book> getAllBooks() {
        try {
            return this.server_obj.getAllBooks();
        } catch (Exception e) {
            System.err.println("Error!\n - " + e);
        }

        return null;
    }

}