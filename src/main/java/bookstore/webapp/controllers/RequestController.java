package bookstore.webapp.controllers;

import javax.servlet.http.*;

import bookstore.webapp.beans.*;
import bookstore.webapp.database.DBConnection;

import java.io.IOException;
import java.util.Enumeration;

import javax.servlet.ServletException;
import javax.servlet.annotation.WebServlet;

@WebServlet(name = "RequestController", urlPatterns = "/request")
public class RequestController extends HttpServlet {
    private static final long serialVersionUID = -7012574937926535948L;

    @Override
    protected void doGet(HttpServletRequest req, HttpServletResponse res) throws ServletException, IOException {
        Enumeration<String> names = req.getParameterNames();
        while (names.hasMoreElements()) {
            System.out.println(names.nextElement());
        }
    }
}