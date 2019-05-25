package bookstore.server;

import java.util.Properties;

import javax.mail.Authenticator;
import javax.mail.Message;
import javax.mail.PasswordAuthentication;
import javax.mail.Session;
import javax.mail.Transport;
import javax.mail.internet.InternetAddress;
import javax.mail.internet.MimeMessage;

class EmailDispatcher {
    private static final String USERNAME = "bookstoreproj2@gmail.com";
    private static final String PASSWORD = "Pass123!";


    private static Properties setupProperties() {
        Properties props = new Properties();
        props.setProperty("mail.transport.protocol", "smtp");
        props.setProperty("mail.smtp.host", "smtp.gmail.com");
        props.setProperty("mail.smtp.port", "587");
        props.setProperty("mail.smtp.auth", "true");
        props.put("mail.smtp.starttls.enable", "true");

        return props;
    }

    private static Authenticator setupAuthenticator() {
        return new Authenticator() {
            @Override
            protected PasswordAuthentication getPasswordAuthentication() {
                return new PasswordAuthentication(USERNAME, PASSWORD);
            }
        };
    }

    static boolean sendEmail(String to, String subject, String body) {
        Session session = Session.getDefaultInstance(setupProperties(), setupAuthenticator());

        try {
            Message msg = new MimeMessage(session);
            msg.setFrom(new InternetAddress(USERNAME));
            msg.addRecipient(Message.RecipientType.TO, new InternetAddress(to));
            msg.setSubject(subject);
            msg.setText(body);

            Transport.send(msg);
            System.out.println("Sent email to '" + to + "'");
            return true;
        }
        catch (Exception e) {
            System.err.println("Failed to send email to '" + to + "'\n - " + e.getMessage());
            return false;
        }
    }
}