package bookstore;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.SQLException;
import java.util.LinkedList;

/**
 *
 * @author jalmeida
 */
public class BookstoreServer {
    private static final String DB_URL = "jdbc:mysql://localhost:3306/bookstore?useJDBCCompliantTimezoneShift=true&useLegacyDatetimeCode=false&serverTimezone=UTC";
    private static final String USERNAME = "root";
    private static final String PASSWORD = "Pass123!";
   
   private final String USER_REQUESTS_QUERY = "SELECT Request.book_title FROM Request, SingleOrder "
           + "WHERE SingleOrder.client_name = ? AND SingleOrder.id = Request.order_id;";
   
   private final Connection conn;
   
   private String username;
   private String email;
   
   public BookstoreServer() throws SQLException {
        this.conn = DriverManager.getConnection(DB_URL, USERNAME, PASSWORD);
        this.conn.setAutoCommit(false);
   }
   
   public LinkedList<String> getUserRequests(String username) throws SQLException {
        Connection con = null;
        PreparedStatement pstmt;
        System.out.println("Getting requests for user " + username);
        try {
                pstmt = this.conn.prepareStatement(USER_REQUESTS_QUERY);
                pstmt.setNString(1, username);
                LinkedList<String> books = (LinkedList<String>)pstmt.executeQuery().getObject(1);

                this.conn.commit();
                pstmt.close();
                return books;
        } finally {
                if (con != null) con.close();
        }
   }
}
