package bookstore.server.responses;

import java.io.Serializable;
import java.util.Calendar;
import java.util.Date;

public class BookOrder implements Serializable {
    private static final long serialVersionUID = 7899496441518523626L;
    private String title;
    private int amount;
    private Date disp_date;
    private String state;

    public BookOrder(String title, int amount, Date date, String state) {
        this.title = title;
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
        return title;
    }

    public void setAwaitingState() {
        this.state = "Waiting Expedition";
    }

    public void setDispatchedAt(boolean dispatched, int n_days) {
        this.state = (dispatched ? "Dispatched at" : "Dispatch will occur at");
        Date dt = new Date();
        Calendar c = Calendar.getInstance();
        c.add(Calendar.DATE, n_days);
        this.disp_date = c.getTime();

    }

    public void setDate(Date date) {
        this.disp_date = date;
    }

    public void setState(String state) {
        this.state = state;
    }
}