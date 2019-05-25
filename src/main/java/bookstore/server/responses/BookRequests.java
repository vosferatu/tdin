package bookstore.server.responses;

import java.util.LinkedList;

public class BookRequests {
    private String title;
    private int amount;
    private LinkedList<Long> reqs;

    public BookRequests(String title, int amount, LinkedList<Long> reqs_uuid) {
        this.title = title;
        this.amount = amount;
        this.reqs = reqs_uuid;
    }

    public String getTitle() {
        return this.title;
    }

    public int getAmount() {
        return this.amount;
    }

    public LinkedList<Long> getReqsID() {
        return this.reqs;
    }
}