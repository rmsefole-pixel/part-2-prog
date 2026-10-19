package za.ac.chatapp;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertTrue;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

/**
 * Unit tests for the Login class.
 *
 * The test data follows the Part 1 PoE brief.
 */
public class LoginTest {

    private Login login;

    @BeforeEach
    public void setUp() {
        login = new Login("Kyle", "Smith");
        login.registerUser("kyl_1", "Ch&&sec@ke99!", "+27838968976");
    }

    @Test
    public void testUsernameCorrectlyFormatted() {
        assertEquals(true, login.checkUserName("kyl_1"));
    }

    @Test
    public void testUsernameIncorrectlyFormatted() {
        assertEquals(false, login.checkUserName("kyle!!!!!!!"));
    }

    @Test
    public void testPasswordMeetsComplexityRequirements() {
        assertEquals(
                "User registered successfully.",
                login.registerUser("kyl_1", "Ch&&sec@ke99!", "+27838968976")
        );
    }

    @Test
    public void testPasswordDoesNotMeetComplexityRequirements() {
        assertEquals(
                "Password is not correctly formatted; please ensure that the password contains at least eight characters, a capital letter, a number, and a special character.",
                login.registerUser("kyl_1", "password", "+27838968976")
        );
    }

    @Test
    public void testCellPhoneCorrectlyFormatted() {
        assertEquals(true, login.checkCellPhoneNumber("+27838968976"));
    }

    @Test
    public void testCellPhoneIncorrectlyFormatted() {
        assertEquals(
                "Cell number is incorrectly formatted or does not contain an international code; please correct the number and try again.",
                login.registerUser("kyl_1", "Ch&&sec@ke99!", "08966553")
        );
    }

    @Test
    public void testLoginSuccessful() {
        assertTrue(login.loginUser("kyl_1", "Ch&&sec@ke99!"));
    }

    @Test
    public void testLoginFailed() {
        assertFalse(login.loginUser("kyl_1", "WrongPassword1!"));
    }

    @Test
    public void testUsernameCorrectlyFormattedUsingAssertTrue() {
        assertTrue(login.checkUserName("kyl_1"));
    }

    @Test
    public void testUsernameIncorrectlyFormattedUsingAssertFalse() {
        assertFalse(login.checkUserName("kyle!!!!!!!"));
    }

    @Test
    public void testPasswordComplexityUsingAssertTrue() {
        assertTrue(login.checkPasswordComplexity("Ch&&sec@ke99!"));
    }

    @Test
    public void testPasswordComplexityUsingAssertFalse() {
        assertFalse(login.checkPasswordComplexity("password"));
    }

    @Test
    public void testCellPhoneUsingAssertTrue() {
        assertTrue(login.checkCellPhoneNumber("+27838968976"));
    }

    @Test
    public void testCellPhoneUsingAssertFalse() {
        assertFalse(login.checkCellPhoneNumber("08966553"));
    }

    @Test
    public void testSuccessfulLoginStatus() {
        login.loginUser("kyl_1", "Ch&&sec@ke99!");

        assertEquals(
                "Welcome Kyle Smith, it is great to see you again.",
                login.returnLoginStatus()
        );
    }

    @Test
    public void testFailedLoginStatus() {
        login.loginUser("kyl_1", "incorrect");

        assertEquals(
                "Username or password incorrect, please try again.",
                login.returnLoginStatus()
        );
    }
}
