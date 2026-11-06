import java.io.FileWriter;
import java.io.IOException;
import java.util.Random;

public class Message {
    private String messageID;
    private int msgSentNumber;
    private String recipient;
    private String messageText;
    private String messageHash;
    private String flag; // "Sent", "Stored", "Disregarded"

    public Message(int msgSentNumber, String recipient, String messageText, String flag) {
        this.msgSentNumber = msgSentNumber;
        this.recipient = recipient;
        this.messageText = messageText;
        this.flag = flag;
        this.messageID = generateMessageID();
        this.messageHash = createMessageHash();
    }

    private String generateMessageID() {
        Random random = new Random();
        long id = 1000000000L + (long)(random.nextDouble() * 9000000000L);
        return String.valueOf(id);
    }

    public boolean checkMessageID(String id) {
        return id != null && id.length() <= 10;
    }

    public boolean checkRecipientCell(String recipientCell) {
        if (recipientCell == null) return false;
        // Must contain international code (e.g. starts with +) and length <= 10 or standard ZA format (+27...)
        return recipientCell.startsWith("+") && recipientCell.length() <= 12;
    }

    public boolean checkMessageLength(String text) {
        return text != null && text.length() <= 250;
    }

    public String createMessageHash() {
        if (messageID == null || messageID.length() < 2 || messageText == null) {
            return "00:0:UNKNOWN";
        }
        String firstTwoId = messageID.substring(0, 2);
        
        String trimmed = messageText.trim();
        String[] words = trimmed.split("\\s+");
        String firstWord = words.length > 0 ? words[0].replaceAll("[^a-zA-Z]", "") : "";
        String lastWord = words.length > 0 ? words[words.length - 1].replaceAll("[^a-zA-Z]", "") : "";

        return (firstTwoId + ":" + msgSentNumber + ":" + firstWord + lastWord).toUpperCase();
    }

    /**
     * Method to store message into a JSON format file using ChatGPT-assisted implementation structure.
     * Attribution / Reference (APA format for AI tools):
     * OpenAI. (2023). ChatGPT (Dec 14 version) [Large language model]. https://chat.openai.com
     */
    public void storeMessageJSON(String filename) {
        try (FileWriter file = new FileWriter(filename, true)) {
            String jsonEntry = "{\n" +
                    "  \"messageID\": \"" + messageID + "\",\n" +
                    "  \"msgSentNumber\": " + msgSentNumber + ",\n" +
                    "  \"recipient\": \"" + recipient + "\",\n" +
                    "  \"messageText\": \"" + messageText + "\",\n" +
                    "  \"messageHash\": \"" + messageHash + "\",\n" +
                    "  \"flag\": \"" + flag + "\"\n" +
                    "},\n";
            file.write(jsonEntry);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    // Getters
    public String getMessageID() { return messageID; }
    public int getMsgSentNumber() { return msgSentNumber; }
    public String getRecipient() { return recipient; }
    public String getMessageText() { return messageText; }
    public String getMessageHash() { return messageHash; }
    public String getFlag() { return flag; }
}