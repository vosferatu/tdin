package bookstore.beans;

import java.util.Date;

public class BookOrder {
    private String title;
    private int amount;
    private Date disp_date;
    private String state;

    BookOrder(String title, int amount, Date date, String state) {
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
}