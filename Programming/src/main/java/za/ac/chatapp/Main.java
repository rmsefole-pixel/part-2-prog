package za.ac.chatapp;

import java.util.Scanner;

/**
 * Console application for Part 1 of the Chat App PoE.
 */
public class Main {

    public static void main(String[] args) {
        Scanner scanner = new Scanner(System.in);
        Login login = new Login();

        System.out.println("======================================");
        System.out.println("        CHAT APP - PART 1");
        System.out.println("     Registration and Login");
        System.out.println("======================================");

        System.out.print("Enter your first name: ");
        String firstName = scanner.nextLine();

        System.out.print("Enter your last name: ");
        String lastName = scanner.nextLine();

        login.setUserDetails(firstName, lastName);

        System.out.println("\n--- REGISTRATION ---");

        System.out.print("Enter username: ");
        String username = scanner.nextLine();

        if (login.checkUserName(username)) {
            System.out.println("Username successfully captured.");
        } else {
            System.out.println("Username is not correctly formatted; please ensure that your username contains an underscore and is no more than five characters in length.");
        }

        System.out.print("Enter password: ");
        String password = scanner.nextLine();

        if (login.checkPasswordComplexity(password)) {
            System.out.println("Password successfully captured.");
        } else {
            System.out.println("Password is not correctly formatted; please ensure that the password contains at least eight characters, a capital letter, a number, and a special character.");
        }

        System.out.print("Enter South African cell phone number (e.g. +27838968976): ");
        String cellPhone = scanner.nextLine();

        if (login.checkCellPhoneNumber(cellPhone)) {
            System.out.println("Cell phone number successfully added.");
        } else {
            System.out.println("Cell phone number incorrectly formatted or does not contain international code.");
        }

        String registrationMessage =
                login.registerUser(username, password, cellPhone);

        System.out.println("\n" + registrationMessage);

        if (registrationMessage.equals("User registered successfully.")) {
            System.out.println("\n--- LOGIN ---");

            System.out.print("Enter username: ");
            String loginUsername = scanner.nextLine();

            System.out.print("Enter password: ");
            String loginPassword = scanner.nextLine();

            if (login.loginUser(loginUsername, loginPassword)) {
                System.out.println(login.returnLoginStatus());
            } else {
                System.out.println(login.returnLoginStatus());
            }
        }

        scanner.close();
    }
}
