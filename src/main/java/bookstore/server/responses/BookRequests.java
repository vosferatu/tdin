package bookstore.server.responses;

import java.io.Serializable;
import java.util.LinkedList;

public class BookRequests implements Serializable {
    private static final long serialVersionUID = -3951350722254221596L;
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

    public boolean hasID(long id) {
        return this.reqs.contains(id);
    }

    @Override
    public String toString() {
        return String.format("Title = %s, Amount = %d, IDS=" + reqs.toString(), this.title, this.amount);
    }
}