package za.ac.chatapp;

import java.util.regex.Pattern;

/**
 * Handles registration validation and login functionality.
 */
public class Login {

    private String registeredUsername;
    private String registeredPassword;
    private String firstName;
    private String lastName;
    private String cellPhoneNumber;

    private boolean lastLoginSuccessful;

    public Login() {
        // Default constructor.
    }

    public Login(String firstName, String lastName) {
        this.firstName = firstName;
        this.lastName = lastName;
    }

    /**
     * Checks that a username contains an underscore and is no more than
     * five characters long.
     *
     * @param username username to validate
     * @return true when correctly formatted
     */
    public boolean checkUserName(String username) {
        return username != null
                && username.length() <= 5
                && username.contains("_");
    }

    /**
     * Checks password complexity.
     * The password must contain at least 8 characters, one uppercase
     * letter, one number and one special character.
     *
     * @param password password to validate
     * @return true when password meets all requirements
     */
    public boolean checkPasswordComplexity(String password) {
        if (password == null || password.length() < 8) {
            return false;
        }

        boolean hasCapital = false;
        boolean hasNumber = false;
        boolean hasSpecial = false;

        for (char character : password.toCharArray()) {
            if (Character.isUpperCase(character)) {
                hasCapital = true;
            } else if (Character.isDigit(character)) {
                hasNumber = true;
            } else if (!Character.isLetterOrDigit(character)) {
                hasSpecial = true;
            }
        }

        return hasCapital && hasNumber && hasSpecial;
    }

    /**
     * Validates a South African cell phone number in international format.
     *
     * The format used is +27 followed by exactly 9 digits, giving the
     * international representation of a 10-digit South African mobile
     * number, e.g. +27838968976.
     *
     * The regular expression approach is based on the standard Java
     * Pattern/Matcher regular-expression mechanism:
     * https://docs.oracle.com/en/java/javase/17/docs/api/java.base/java/util/regex/Pattern.html
     *
     * @param cellPhoneNumber number to validate
     * @return true when correctly formatted
     */
    public boolean checkCellPhoneNumber(String cellPhoneNumber) {
        if (cellPhoneNumber == null) {
            return false;
        }

        String regex = "^\\+27[0-9]{9}$";
        return Pattern.matches(regex, cellPhoneNumber);
    }

    /**
     * Registers a user after validating the username, password and
     * cellphone number.
     *
     * @return registration status message
     */
    public String registerUser(String username, String password,
                                String cellPhoneNumber) {

        if (!checkUserName(username)) {
            return "Username is not correctly formatted; please ensure that your username contains an underscore and is no more than five characters in length.";
        }

        if (!checkPasswordComplexity(password)) {
            return "Password is not correctly formatted; please ensure that the password contains at least eight characters, a capital letter, a number, and a special character.";
        }

        if (!checkCellPhoneNumber(cellPhoneNumber)) {
            return "Cell number is incorrectly formatted or does not contain an international code; please correct the number and try again.";
        }

        registeredUsername = username;
        registeredPassword = password;
        this.cellPhoneNumber = cellPhoneNumber;

        return "User registered successfully.";
    }

    /**
     * Stores the user's name for the successful-login message.
     */
    public void setUserDetails(String firstName, String lastName) {
        this.firstName = firstName;
        this.lastName = lastName;
    }

    /**
     * Verifies that supplied login details match the registered details.
     *
     * @param username login username
     * @param password login password
     * @return true for a successful login
     */
    public boolean loginUser(String username, String password) {
        lastLoginSuccessful = registeredUsername != null
                && registeredPassword != null
                && registeredUsername.equals(username)
                && registeredPassword.equals(password);

        return lastLoginSuccessful;
    }

    /**
     * Returns the required login status message.
     *
     * @return welcome message or failed-login message
     */
    public String returnLoginStatus() {
        if (lastLoginSuccessful) {
            String safeFirstName = firstName == null ? "" : firstName;
            String safeLastName = lastName == null ? "" : lastName;

            return "Welcome " + safeFirstName + " " + safeLastName
                    + ", it is great to see you again.";
        }

        return "Username or password incorrect, please try again.";
    }

    public String getRegisteredUsername() {
        return registeredUsername;
    }

    public String getCellPhoneNumber() {
        return cellPhoneNumber;
    }
}
