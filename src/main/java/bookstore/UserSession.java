/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package bookstore;

import java.sql.SQLException;
import java.util.LinkedList;

import bookstore.beans.Request;
import bookstore.beans.Book;
/**
 *
 * @author jalmeida
 */
public class UserSession {
    private String username;
    private String email;

    private BookstoreServer server;
    
    public UserSession() throws SQLException {
        this.server = new BookstoreServer();
    }
    
    /**
     * @return the username
     */
    public String getUsername() {
        return username;
    }

    /**
     * @param username the username to set
     */
    public void setUsername(String username) {
        this.username = username;
    }

    /**
     * @return the email
     */
    public String getEmail() {
        return email;
    }

    /**
     * @param email the email to set
     */
    public void setEmail(String email) {
        this.email = email;
    }
  
    public String getUserBooks() {
        try {
            if (this.server != null) {
                Request req = this.server.getUserRequests(this.username);
                return req.toHTMLTable();
            }
        }
        catch (SQLException e) {
            System.err.println("Error!\n - " + e);
        }
        
        return null;
    }  

    public String getAllBooks() {
        try {
            if (this.server != null) {
                LinkedList<Book> books = this.server.getAllBooks();
                String html = "";
                for (Book book : books) {
                    html += book.toHTML();
                }
                return html;
            }
        }
        catch (SQLException e) {
            System.err.println("Error!\n - " + e);
        }

        return null;
    }
}
