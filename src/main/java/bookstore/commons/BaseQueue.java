package bookstore.commons;

import com.rabbitmq.client.ConnectionFactory;
import com.rabbitmq.client.DeliverCallback;
import com.rabbitmq.client.Connection;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.io.Serializable;
import java.util.concurrent.TimeoutException;

import com.rabbitmq.client.CancelCallback;
import com.rabbitmq.client.Channel;

public class BaseQueue {
    private static final String HOST = "localhost";
    protected final String QUEUE_NAME;

    protected Connection conn;
    protected Channel channel;

    public BaseQueue(String queue_name) throws IOException, TimeoutException {
        this.QUEUE_NAME = queue_name;
        ConnectionFactory factory = new ConnectionFactory();
        factory.setHost(HOST);
        this.conn = factory.newConnection();
        this.channel = this.conn.createChannel();
        this.channel.queueDeclare(queue_name, true, false, false, null);
    }

    byte[] objToBytes(Serializable obj) {
        byte[] bytes = null;
        ByteArrayOutputStream baos = new ByteArrayOutputStream();
        try {
            ObjectOutputStream oos = new ObjectOutputStream(baos);
            oos.writeObject(obj);
            oos.flush();
            oos.reset();
            bytes = baos.toByteArray();
            oos.close();
            baos.close();
            return bytes;
        }
        catch (Exception err) {
            System.err.println("Cannot turn object into bytes!\n - " + err);
            return null;
        }
    }

    public Serializable objFromBytes(byte[] bytes) {
        Serializable obj = null;
        try {
            ByteArrayInputStream bis = new ByteArrayInputStream (bytes);
            ObjectInputStream ois = new ObjectInputStream (bis);
            obj = (Serializable)ois.readObject();
            ois.close();
            bis.close();
            return obj;
        }
        catch (Exception err) {
            System.err.println("Cannot turn bytes into object!\n - " + err);
            return null;
        }
    }

    public boolean sendMessage(String msg) {
        try {
            this.channel.basicPublish("", this.QUEUE_NAME, null, msg.getBytes("UTF-8"));
            return true;
        } catch (Exception e) {
            System.out.println("Failed to send '" + msg + "'!\n - " + e);
            return false;
        }
    }

    public boolean sendObject(Serializable obj) {
        try {
            this.channel.basicPublish("", this.QUEUE_NAME, null, this.objToBytes(obj));
            return true;
        } catch (Exception e) {
            System.out.println("Failed to send object!\n - " + e);
            return false;
        }
    }

    public boolean registerCallbacks(DeliverCallback deliver, CancelCallback cancel) {
        try {
            this.channel.basicConsume(this.QUEUE_NAME, deliver, cancel);
            return true;
        }
        catch (Exception e) {
            System.err.println("Failed to register callbacks!\n - " + e);
            return false;
        }
    }
}