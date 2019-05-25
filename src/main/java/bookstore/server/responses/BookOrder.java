package bookstore.server.responses;

import java.io.Serializable;
import java.util.Calendar;
import java.util.Date;

public class BookOrder implements Serializable {
    public static final String WAITING = "Waiting Expedition";
    public static final String DISPATCHED = "Dispatched at";
    public static final String DISPATCHING = "Dispatch should occur at";
    private static final long serialVersionUID = 7899496441518523626L;
    private Book book;
    private int amount;
    private Date disp_date;
    private String state;

    public BookOrder(Book book, int amount, Date date, String state) {
        this.book = book;
        this.amount = amount;
        this.disp_date = date;
        this.state = state;
    }

    public Date getDisp_date() {
        return disp_date;
    }

    public String getState() {
        return state;
    }

    public int getAmount() {
        return amount;
    }

    public String getTitle() {
        return this.book.getTitle();
    }

    public double getPrice() {
        return this.book.getPrice();
    }

    public void setAwaitingState() {
        this.state = WAITING;
    }

    public boolean isWaiting() {
        return this.state.equals(WAITING);
    }

    public void setDispatchedAt(boolean dispatched, int n_days) {
        this.state = (dispatched ? DISPATCHED : DISPATCHING);
        Calendar c = Calendar.getInstance();
        c.add(Calendar.DATE, n_days);
        this.disp_date = c.getTime();

    }

    public boolean isDispatching() {
        return this.state == DISPATCHING;
    }

    public boolean isDispatched() {
        return this.state == DISPATCHED;
    }

    public void setDate(Date date) {
        this.disp_date = date;
    }

    public void setState(String state) {
        this.state = state;
    }
}