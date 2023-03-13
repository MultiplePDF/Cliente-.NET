
const btnRegistration = document.getElementById("register-button");
const btnLogin = document.getElementById("login-button");
const frmLogin = document.getElementById("Login-Form");
const frmRegister = document.getElementById("Register-Form");

//Cambiar de Login a Register
btnRegistration.addEventListener('click', (e) => {
    e.preventDefault();
    frmLogin.style.display = "none";
    frmRegister.style.display = "block";
});

//Cambiar de Register a Login
btnLogin.addEventListener('click', (e) => {
    e.preventDefault();
    frmLogin.style.display = "block";
    frmRegister.style.display = "none";
});
