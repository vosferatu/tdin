package bookstore;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.LinkedList;

import bookstore.beans.Book;
import bookstore.beans.Request;

/**
 *
 * @author jalmeida
 */
public class BookstoreServer {
    private static final String DB_URL = "jdbc:mysql://localhost:3306/bookstore?useJDBCCompliantTimezoneShift=true&useLegacyDatetimeCode=false&serverTimezone=UTC";
    private static final String USERNAME = "root";
    private static final String PASSWORD = "Pass123!";

    private final String USER_REQUESTS_QUERY = 
        "SELECT Request.book_title AS title, Request.amount, Request.disp_date AS date, Request.req_state"
        + " FROM Request, SingleOrder"
        + " WHERE SingleOrder.client_name = ? AND SingleOrder.id = Request.order_id;";
    private final String ALL_BOOKS_QUERY = "SELECT * FROM Book;";

    private final Connection conn;

    public BookstoreServer() throws SQLException {
        this.conn = DriverManager.getConnection(DB_URL, USERNAME, PASSWORD);
        this.conn.setAutoCommit(false);
    }

    public Request getUserRequests(String username) throws SQLException {
        System.out.println("Getting requests for user " + username);
        
        PreparedStatement pstmt = this.conn.prepareStatement(USER_REQUESTS_QUERY);
        pstmt.setNString(1, username);
        
        ResultSet res = pstmt.executeQuery();
        Request req = new Request(res);

        this.conn.commit();
        pstmt.close();

        return req;   
    }

    public LinkedList<Book> getAllBooks() throws SQLException {
        System.out.println("Getting all store books");
        PreparedStatement pstmt = this.conn.prepareStatement(ALL_BOOKS_QUERY);
        ResultSet res = pstmt.executeQuery();
        this.conn.commit();
        
        LinkedList<Book> books = new LinkedList<>();
        while (res.next()) {
            books.add(new Book(res.getString("title"), res.getDouble("price"), res.getInt("stock")));
        }
        
        pstmt.close();
        return books;
    }
}
