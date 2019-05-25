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

import javax.mail.internet.InternetAddress;
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
        private final String addr;

        private Client(String user, String email, String address) {
            this.username = user;
            this.email = email;
            this.addr = address;
        }

        public static Client fromReqAttributes(HttpServletRequest req) {
            return new Client(req.getParameter("username"), req.getParameter("email"), req.getParameter("address"));
        }

        public void setAsReqAttributes(HttpServletRequest req) {
            req.setAttribute("username", this.username);
            req.setAttribute("email", this.email);
            req.setAttribute("address", this.addr);
        }

        public List<String> validate() {
            List<String> errors = new ArrayList<>();
            if (this.username.length() == 0) {
                errors.add("Username not valid!");
            }
            boolean valid_email = true;
            try {
                InternetAddress email_addr = new InternetAddress(this.email);
                email_addr.validate();
            } catch (Exception err) {
                valid_email = false;
            }
            if (!valid_email) {
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
            String username = req.getParameter("username");
            LinkedList<Request> requests = this.server_obj.getUserRequests(username);
            req.setAttribute("requests", requests);
            req.setAttribute("all_books", this.getAllBooks());
        }

        req.getRequestDispatcher(url).forward(req, res);
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