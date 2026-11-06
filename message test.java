import org.junit.jupiter.api.Test;
import static org.junit.jupiter.api.Assertions.*;

public class MessageTest {

    @Test
    public void testMessageLengthSuccess() {
        Message msg = new Message(1, "+27718693002", "Hi Mike, can you join us for dinner tonight", "Sent");
        assertTrue(msg.checkMessageLength(msg.getMessageText()));
    }

    @Test
    public void testRecipientFormatting() {
        Message msg = new Message(1, "+27718693002", "Test", "Sent");
        assertTrue(msg.checkRecipientCell(msg.getRecipient()));
    }

    @Test
    public void testMessageHashCreation() {
        Message msg = new Message(1, "+27718693002", "Hi Mike, can you join us for dinner tonight", "Sent");
        assertNotNull(msg.createMessageHash());
    }

    @Test
    public void testMessageIDGeneration() {
        Message msg = new Message(1, "+27718693002", "Test ID", "Sent");
        assertTrue(msg.checkMessageID(msg.getMessageID()));
    }
}