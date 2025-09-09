$(function () {
    const $loginForm = $("#loginForm");
    const $registerForm = $("#registerForm");
    const $loginError = $("#error-summary-login");
    const $registerError = $("#error-summary-register");
    const $toggleLink = $("#toggle-link");
    const $toggleText = $("#toggle-text");
    const $loginBlock = $("#login-form");
    const $registerBlock = $("#register-form");

    // Toggle view functions
    function showLogin() {
        $loginBlock.show();
        $registerBlock.hide();
        $toggleText.text("Don't have an account?");
        $toggleLink.text("Register");
        localStorage.setItem("currentView", "login");
    }

    function showRegister() {
        $loginBlock.hide();
        $registerBlock.show();
        $toggleText.text("Already have an account?");
        $toggleLink.text("Sign In");
        localStorage.setItem("currentView", "register");
    }

    // Toggle link event handler
    $toggleLink.on("click", function (e) {
        e.preventDefault();
        if ($loginBlock.is(":visible")) {
            showRegister();
        } else {
            showLogin();
        }
    });

    // Initialize view
    (function initView() {
        const serverView = $("#form-container").data("view");
        const saved = localStorage.getItem("currentView");
        if (serverView === "register" || saved === "register") {
            showRegister();
        } else {
            showLogin();
        }
        localStorage.removeItem("currentView");
    })();

    // Helper functions
    function showError($container, message) {
        const html = `<div class="alert alert-danger">${message}</div>`;
        $container.html(html);
    }

    function clearError($container) {
        $container.html("");
    }

    function ajaxPost($form, successCallback, errorCallback) {
        const url = $form.attr("action") || window.location.href;
        const data = $form.serialize();

        $.ajax({
            url: url,
            method: "POST",
            data: data,
            beforeSend: function () {
                // Disable submit button to prevent double submission
                $form.find('button[type="submit"]').prop('disabled', true);
            },
            success: function (res) {
                if (typeof successCallback === "function") successCallback(res);
            },
            error: function (xhr) {
                if (typeof errorCallback === "function") errorCallback(xhr);
            },
            complete: function () {
                // Re-enable submit button
                $form.find('button[type="submit"]').prop('disabled', false);
            }
        });
    }

    // LOGIN FORM HANDLER
    $loginForm.on("submit", function (e) {
        e.preventDefault();
        clearError($loginError);

        const email = $.trim($loginForm.find('input[name="Login.Email"]').val());
        const password = $.trim($loginForm.find('input[name="Login.Password"]').val());

        // Client-side validation
        if (!email || !password) {
            showError($loginError, "Please enter both email and password.");
            return;
        }

        // Email format validation
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(email)) {
            showError($loginError, "Please enter a valid email address.");
            return;
        }

        ajaxPost($loginForm,
            function (res) {
                if (res && res.success) {
                    if (res.redirectUrl) {
                        window.location.href = res.redirectUrl;
                    } else {
                        location.reload();
                    }
                } else {
                    showError($loginError, res && res.message ? res.message : "Invalid email or password.");
                }
            },
            function (xhr) {
                let errorMessage = "Something went wrong while logging in. Try again.";
                if (xhr.responseJSON && xhr.responseJSON.message) {
                    errorMessage = xhr.responseJSON.message;
                }
                showError($loginError, errorMessage);
            }
        );
    });

    // REGISTER FORM HANDLER
    $registerForm.on("submit", function (e) {
        e.preventDefault();
        clearError($registerError);

        const username = $.trim($registerForm.find('input[name="Register.UserName"]').val());
        const email = $.trim($registerForm.find('input[name="Register.Email"]').val());
        const pass = $.trim($registerForm.find('input[name="Register.Password"]').val());
        const confirm = $.trim($registerForm.find('input[name="Register.ConfirmPassword"]').val());

        // Client-side validation
        if (!username || !email || !pass || !confirm) {
            showError($registerError, "Please fill all required fields.");
            return;
        }

        if (username.length < 3) {
            showError($registerError, "Username must be at least 3 characters long.");
            return;
        }

        // Email format validation
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(email)) {
            showError($registerError, "Please enter a valid email address.");
            return;
        }

        if (pass.length < 6) {
            showError($registerError, "Password must be at least 6 characters long.");
            return;
        }

        if (pass !== confirm) {
            showError($registerError, "Passwords do not match.");
            return;
        }

        ajaxPost($registerForm,
            function (res) {
                if (res && res.success) {
                    if (res.redirectUrl) {
                        window.location.href = res.redirectUrl;
                    } else {
                        location.reload();
                    }
                } else {
                    showError($registerError, res && res.message ? res.message : "Registration failed.");
                }
            },
            function (xhr) {
                let errorMessage = "Something went wrong while registering. Try again.";
                if (xhr.responseJSON && xhr.responseJSON.message) {
                    errorMessage = xhr.responseJSON.message;
                }
                showError($registerError, errorMessage);
            }
        );
    });

    // Real-time validation for better UX
    $loginForm.find('input[name="Login.Email"]').on('blur', function () {
        const email = $.trim($(this).val());
        if (email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
            $(this).addClass('is-invalid');
        } else {
            $(this).removeClass('is-invalid');
        }
    });

    $registerForm.find('input[name="Register.Email"]').on('blur', function () {
        const email = $.trim($(this).val());
        if (email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
            $(this).addClass('is-invalid');
        } else {
            $(this).removeClass('is-invalid');
        }
    });

    $registerForm.find('input[name="Register.ConfirmPassword"]').on('keyup', function () {
        const password = $.trim($registerForm.find('input[name="Register.Password"]').val());
        const confirmPassword = $.trim($(this).val());

        if (confirmPassword && password !== confirmPassword) {
            $(this).addClass('is-invalid');
        } else {
            $(this).removeClass('is-invalid');
        }
    });

    $registerForm.find('input[name="Register.Password"]').on('keyup', function () {
        const password = $.trim($(this).val());
        const confirmPassword = $.trim($registerForm.find('input[name="Register.ConfirmPassword"]').val());

        // Check password strength
        if (password && password.length >= 6) {
            $(this).removeClass('is-invalid').addClass('is-valid');
        } else if (password) {
            $(this).removeClass('is-valid').addClass('is-invalid');
        } else {
            $(this).removeClass('is-valid is-invalid');
        }

        // Check if confirm password still matches
        if (confirmPassword && password !== confirmPassword) {
            $registerForm.find('input[name="Register.ConfirmPassword"]').addClass('is-invalid');
        } else if (confirmPassword) {
            $registerForm.find('input[name="Register.ConfirmPassword"]').removeClass('is-invalid');
        }
    });
});