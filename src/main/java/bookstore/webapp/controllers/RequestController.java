package bookstore.webapp.controllers;

import javax.servlet.http.*;

import bookstore.commons.BaseRMI;
import bookstore.server.ServerInterface;
import bookstore.server.responses.Request;

import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.util.Enumeration;
import java.util.HashMap;
import java.util.Iterator;

import javax.json.Json;
import javax.json.JsonArray;
import javax.json.JsonException;
import javax.json.JsonObject;
import javax.json.JsonReader;
import javax.servlet.ServletException;
import javax.servlet.annotation.WebServlet;

@WebServlet(name = "RequestController", urlPatterns = "/request")
public class RequestController extends HttpServlet {
    private static final long serialVersionUID = -7012574937926535948L;
    private static final String OBJ_NAME = "BookstoreServer";
    private static final int OBJ_PORT = 8005;

    ServerInterface server_obj;

    public RequestController() {
        this.server_obj = (ServerInterface)BaseRMI.fetchObject(OBJ_NAME, OBJ_PORT);
    }

    @Override
    protected void doGet(HttpServletRequest req, HttpServletResponse res) throws ServletException, IOException {
        HashMap<String, Integer> book_amounts = new HashMap<>();
        JsonReader json_reader = Json.createReader(new ByteArrayInputStream(req.getParameter("books").getBytes()));
        HashMap<String, Integer> books = jsonToMap(json_reader.readObject());
        String name = req.getParameter("username"), email = req.getParameter("email"), addr = req.getParameter("address");

        this.server_obj.putRequest(name, addr, email, books);

        res.setStatus(HttpServletResponse.SC_OK);
    }

    public static HashMap<String, Integer> jsonToMap(JsonObject json) throws JsonException {
        HashMap<String, Integer> retMap = new HashMap<String, Integer>();

        if (json != JsonObject.NULL) {
            retMap = toMap(json);
        }
        return retMap;
    }

    public static HashMap<String, Integer> toMap(JsonObject object) throws JsonException {
        HashMap<String, Integer> map = new HashMap<String, Integer>();

        Iterator<String> keysItr = object.keySet().iterator();
        while (keysItr.hasNext()) {
            String key = keysItr.next();
            Integer value = Integer.parseInt(object.get(key).toString());
            map.put(key, value);
        }
        return map;
    }
}