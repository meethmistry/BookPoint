document.addEventListener('DOMContentLoaded', function () {

    // ------- Quantity + View More + Add to Cart code (keep yours here) -------
    // (You can paste all your existing DOM ready code for qty, view-more buttons, fetch add-to-cart, etc.)
    // Example: the quantity handlers you already had:
    document.querySelectorAll('.qty-btn-minus, .qty-btn-plus').forEach(button => {
        button.addEventListener('click', function () {
            const input = this.parentElement.querySelector('.qty-input');
            let value = parseInt(input.value || '1', 10);
            const max = parseInt(input.getAttribute('max') || '999', 10);

            if (this.classList.contains('qty-btn-minus') && value > 1) {
                input.value = value - 1;
            } else if (this.classList.contains('qty-btn-plus') && value < max) {
                input.value = value + 1;
            }
        });
    });

    // ------- LOGOUT CONFIRMATION -------
    const logoutLink = document.getElementById("logoutLink");
    const confirmationOverlay = document.getElementById("confirmationOverlay");
    const cancelLogout = document.getElementById("cancelLogout");
    const confirmLogout = document.getElementById("confirmLogout");
    const logoutForm = document.getElementById("logoutForm");

    // safety: only attach handlers if the elements exist
    if (logoutLink && confirmationOverlay) {
        logoutLink.addEventListener("click", function (e) {
            e.preventDefault();
            confirmationOverlay.style.display = "flex";
            // optionally trap focus here if needed
        });
    }

    if (cancelLogout && confirmationOverlay) {
        cancelLogout.addEventListener("click", function () {
            confirmationOverlay.style.display = "none";
        });
    }

    if (confirmLogout && logoutForm) {
        // Instead of redirecting (GET), submit the hidden POST form (with antiforgery token)
        confirmLogout.addEventListener("click", function () {
            logoutForm.submit();
        });
    }

    // click outside the card to close
    if (confirmationOverlay) {
        confirmationOverlay.addEventListener('click', function (e) {
            if (e.target === confirmationOverlay) {
                confirmationOverlay.style.display = 'none';
            }
        });
    }

    // close with ESC
    document.addEventListener('keydown', function (e) {
        if (e.key === "Escape" && confirmationOverlay && confirmationOverlay.style.display === 'flex') {
            confirmationOverlay.style.display = 'none';
        }
    });

    // ------- Book details modal and Add-to-cart handlers (your existing code) -------
    document.querySelectorAll('.view-more-btn').forEach(button => {
        button.addEventListener('click', function () {
            const title = this.getAttribute('data-book-title') || '';
            const author = this.getAttribute('data-book-author') || '';
            const category = this.getAttribute('data-book-category') || '';
            const price = this.getAttribute('data-book-price') || '';
            const image = this.getAttribute('data-book-image') || '';
            const description = this.getAttribute('data-book-description') || '';
            const samplePdf = this.getAttribute('data-sample-pdf') || '';

            // populate modal
            document.getElementById('modalBookTitle').textContent = title;
            document.getElementById('modalBookAuthor').textContent = `by ${author}`;
            document.getElementById('modalBookCategory').textContent = category;
            document.getElementById('modalBookPrice').textContent = price;
            document.getElementById('modalBookImage').src = image;
            document.getElementById('modalBookDescription').textContent = description;

            const sampleSection = document.getElementById('samplePdfSection');
            const sampleLink = document.getElementById('samplePdfLink');
            if (samplePdf && samplePdf.trim() !== '') {
                sampleSection.style.display = 'block';
                sampleLink.href = samplePdf;
            } else {
                sampleSection.style.display = 'none';
            }

            const modal = new bootstrap.Modal(document.getElementById('bookDetailsModal'));
            modal.show();
        });
    });

    document.querySelectorAll('button[data-book-id]').forEach(button => {
        button.addEventListener('click', function () {
            const bookId = this.getAttribute('data-book-id');
            const qty = this.closest('.book-actions-bottom').querySelector('.qty-input').value;

            fetch('/Customer/AddOrUpdateCart', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ bookId: parseInt(bookId, 10), quantity: parseInt(qty, 10) })
            })
                .then(res => res.json())
                .then(data => {
                    if (data.success) {
                        // ✅ Update message
                        document.querySelector('#cartSuccessOverlay .card-body p.fw-bold')
                            .textContent = data.message || "Book added to cart successfully!";

                        // ✅ Show overlay
                        document.getElementById("cartSuccessOverlay").style.display = "flex";
                    } else {
                        alert(data.message || "Something went wrong!");
                    }
                })
                .catch(err => {
                    console.error(err);
                    alert("Network or server error.");
                });
        });
    });

    // ✅ Close overlay when buttons clicked
    document.getElementById("continueShopping").addEventListener("click", function () {
        document.getElementById("cartSuccessOverlay").style.display = "none";
    });

    document.getElementById("goToCart").addEventListener("click", function () {
        window.location.href = "/Cart/Index"; 
    });



    const profileOverlay = document.getElementById("profileOverlay");
    const cancelProfile = document.getElementById("cancelProfile");
    const updateProfile = document.getElementById("updateProfile");

    // Show Profile Card on click
    document.querySelector(".nav-links a:has(i.fa-user)")?.addEventListener("click", function (e) {
        e.preventDefault();

        if (!currentUID || currentUID === 0) {
            alert("User not logged in.");
            return;
        }

        // Fetch profile details for current user
        fetch(`/Customer/GetCustomerProfile?uid=${currentUID}`)
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    document.getElementById("profileUID").value = currentUID;
                    document.getElementById("profileUserName").value = data.data.userName;
                    document.getElementById("profileEmail").textContent = data.data.email;
                    document.getElementById("profilePhone").value = data.data.phone;
                    document.getElementById("profileAddress").value = data.data.address;
                }
                profileOverlay.style.display = "flex";
            })
            .catch(err => console.error("Error loading profile:", err));
    });

    // Cancel button
    cancelProfile.addEventListener("click", function () {
        profileOverlay.style.display = "none";
    });


    updateProfile.addEventListener("click", function () {
        const uid = document.getElementById("profileUID").value;
        const userName = document.getElementById("profileUserName").value.trim();
        const phone = document.getElementById("profilePhone").value.trim();
        const address = document.getElementById("profileAddress").value.trim();
        const messageBox = document.getElementById("profileMessage");

        // Clear old messages
        messageBox.innerHTML = "";

        // 🔹 Validation
        if (!userName) {
            messageBox.innerHTML = `<span class="text-danger">Username is required.</span>`;
            return;
        }

        if (phone && !/^\d{10}$/.test(phone)) {
            messageBox.innerHTML = `<span class="text-danger">Phone number must be exactly 10 digits.</span>`;
            return;
        }

        const payload = {
            UID: parseInt(uid),
            UserName: userName,
            Phone: phone,
            Address: address
        };

        fetch("/Customer/UpdateCustomerProfile", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        })
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    messageBox.innerHTML = `<span class="text-success">Profile updated successfully.</span>`;
                    // Close card after short delay
                    setTimeout(() => {
                        profileOverlay.style.display = "none";
                    }, 1200);
                } else {
                    messageBox.innerHTML = `<span class="text-danger">${data.message || "Update failed."}</span>`;
                }
            })
            .catch(err => {
                console.error("Update error:", err);
                messageBox.innerHTML = `<span class="text-danger">Server error, please try again.</span>`;
            });
    });





});
