import { startAuthentication, startRegistration } from "@simplewebauthn/browser";

const signupButton = document.querySelector("[data-signup]");
const loginButton = document.querySelector("[data-login]");
const emailInput = document.querySelector("[data-email]");
const modal = document.querySelector("[data-modal]");
const closeButton = document.querySelector("[data-close]");

const SERVER_URL = "http://localhost:3000";

signupButton.addEventListener("click", signup);
loginButton.addEventListener("click", login);
closeButton.addEventListener("click", () => modal.close());

async function signup() {
  const email = emailInput.value.trim();

  if (!email) {
    showModalText("Please enter a valid email.");
    return;
  }

  try {
    const initResponse = await fetch(`${SERVER_URL}/init-register?email=${email}`, {
      credentials: "include",
    });
    const options = await initResponse.json();
    if (!initResponse.ok) {
      showModalText(options.error);
      return;
    }

    const registrationJSON = await startRegistration(options);

    const verifyResponse = await fetch(`${SERVER_URL}/verify-register`, {
      credentials: "include",
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(registrationJSON),
    });

    const verifyData = await verifyResponse.json();
    if (!verifyResponse.ok) {
      showModalText(verifyData.error);
      return;
    }

    if (verifyData.verified) {
      showModalText(`Successfully registered ${email}`);

      // Store passkey for 1 year
      savePasskey(email, verifyData.passkey);
    } else {
      showModalText("Failed to register.");
    }
  } catch (error) {
    showModalText("An error occurred during signup.");
  }
}

async function login() {
  const email = emailInput.value.trim();

  if (!email) {
    showModalText("Please enter a valid email.");
    return;
  }

  try {
    const initResponse = await fetch(`${SERVER_URL}/init-auth?email=${email}`, {
      credentials: "include",
    });
    const options = await initResponse.json();
    if (!initResponse.ok) {
      showModalText(options.error);
      return;
    }

    const authJSON = await startAuthentication(options);

    const verifyResponse = await fetch(`${SERVER_URL}/verify-auth`, {
      credentials: "include",
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(authJSON),
    });

    const verifyData = await verifyResponse.json();
    if (!verifyResponse.ok) {
      showModalText(verifyData.error);
      return;
    }

    if (verifyData.verified) {
      showModalText(`Successfully logged in ${email}`);

      // Store passkey for 1 year
      savePasskey(email, verifyData.passkey);

      // Redirect to the dashboard after a short delay
      setTimeout(() => {
        if (email === "admin@admin.com") {
          window.location.href = "http://localhost:5194/AdminDashboard/Index";
        } else {
          showModalText("Proceeding with WebAuthn authentication...");
          window.location.href = "http://localhost:5194/UserDashboard";
        }
      }, 1000);
    } else {
      showModalText("Failed to log in.");
    }
  } catch (error) {
    showModalText("An error occurred during login.");
  }
}

function showModalText(text) {
  modal.querySelector("[data-content]").innerText = text;
  modal.showModal();
}

// Function to store passkey for 1 year
function savePasskey(email, passkey) {
  const expirationTime = Date.now() + 365 * 24 * 60 * 60 * 1000; // 1 year in milliseconds
  const data = { passkey, expires: expirationTime };
  localStorage.setItem(`passkey_${email}`, JSON.stringify(data));
}

// Function to retrieve passkey
function getPasskey(email) {
  const data = JSON.parse(localStorage.getItem(`passkey_${email}`));
  if (data && Date.now() < data.expires) {
    return data.passkey;
  } else {
    localStorage.removeItem(`passkey_${email}`); // Remove expired key
    return null;
  }
}
