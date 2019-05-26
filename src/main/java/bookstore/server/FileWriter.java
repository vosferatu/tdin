package bookstore.server;

import static java.nio.file.StandardOpenOption.CREATE;
import static java.nio.file.StandardOpenOption.WRITE;

import java.nio.ByteBuffer;
import java.nio.channels.AsynchronousFileChannel;
import java.nio.channels.CompletionHandler;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.Map;

class FileWriter {
    protected static int attempts = 0;
    private static final String STOCK_FILE = "assets/database/stock.csv";

    static void writeStocksToFile(Map<String, Integer> stocks) {
        Path path = Paths.get(STOCK_FILE);
        try {
            AsynchronousFileChannel afc = AsynchronousFileChannel.open(path, WRITE);
            WriteHandler write_handler = new WriteHandler();
            ByteBuffer buf = stockToBuffer(stocks);
            Attachment attach = new Attachment();
            attach.channel = afc; 
            attach.buffer = buf;
            attach.path = path;

            afc.write(buf, 0, attach, write_handler);
        }
        catch (Exception e) {
            System.err.println("Failed to write stocks to file!\n - " + e);
        }
    }

    private static ByteBuffer stockToBuffer(Map<String, Integer> stocks) {
        String nl = System.getProperty("line.separator");
        StringBuilder sb = new StringBuilder();
        stocks.forEach((String title, Integer amount) -> {
            sb.append(String.format("%s;%d", title, amount));
            sb.append(nl);
        });
        String str = sb.toString();
        return ByteBuffer.wrap(str.getBytes());
    }

    private static class Attachment {
        public Path path;
        public ByteBuffer buffer;
        public AsynchronousFileChannel channel;
     }

    private static class WriteHandler implements CompletionHandler<Integer, Attachment> {

        @Override
        public void completed(Integer result, Attachment attachment) {
            System.out.println("Dumped stocks to stock.csv!");
        }

        @Override
        public void failed(Throwable exc, Attachment attachment) {
            attempts++;
            System.err.println("Failed to write stocks to stock.csv!\n - " + exc);
            if (attempts <= 3) {
                System.err.println("Retry #" + attempts);
                attachment.channel.write(attachment.buffer, 0, attachment, this);
            }
            else {
                System.err.println("Total failure!");
            }
        }

    }
}