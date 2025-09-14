document.addEventListener('DOMContentLoaded', function () {
    let itemToRemove = null;
    // ------- Quantity Buttons Handler -------
    document.querySelectorAll('.qty-btn-minus, .qty-btn-plus').forEach(button => {
        button.addEventListener('click', function () {
            const input = this.parentElement.querySelector('.qty-input');
            let value = parseInt(input.value || '1', 10);
            const max = parseInt(input.getAttribute('max') || '999', 10);
            const bookId = parseInt(input.getAttribute('data-book-id'), 10);

            if (this.classList.contains('qty-btn-minus') && value > 1) {
                value--;
            } else if (this.classList.contains('qty-btn-plus') && value < max) {
                value++;
            } else {
                return; // no change
            }

            input.value = value;

            // Send update to server
            fetch('/Cart/UpdateCart', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ BookId: bookId, Quantity: value })
            })
                .then(res => res.json())
                .then(data => {
                    if (!data.success) {
                        alert(data.message || "Failed to update cart!");
                    }
                })
                .catch(err => {
                    console.error(err);
                    alert("Network or server error while updating cart.");
                });
        });
    });

    // ------- View More Modal Handler -------
    document.querySelectorAll('.view-more-btn').forEach(button => {
        button.addEventListener('click', function () {
            const title = this.getAttribute('data-book-title') || '';
            const author = this.getAttribute('data-book-author') || '';
            const category = this.getAttribute('data-book-category') || '';
            const price = this.getAttribute('data-book-price') || '';
            const image = this.getAttribute('data-book-image') || '';
            const description = this.getAttribute('data-book-description') || '';
            const samplePdf = this.getAttribute('data-sample-pdf') || '';

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


    // ------- Remove Button Handler with Confirmation -------
    document.querySelectorAll('.remove-from-cart').forEach(button => {
        button.addEventListener('click', function () {
            itemToRemove = this.getAttribute('data-item-id');
            document.getElementById('removeConfirmOverlay').style.display = 'flex';
        });
    });

    document.getElementById('cancelRemove').addEventListener('click', function () {
        itemToRemove = null;
        document.getElementById('removeConfirmOverlay').style.display = 'none';
    });

    document.getElementById('confirmRemove').addEventListener('click', function () {
        if (!itemToRemove) return;
        fetch('/Cart/RemoveItem', {  // You need to implement this action in CartController
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ CartItemId: parseInt(itemToRemove, 10) })
        })
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    // Remove card from DOM
                    const card = document.querySelector(`.remove-from-cart[data-item-id="${itemToRemove}"]`).closest('.book-card-unified');
                    card.remove();
              } else {
                    alert(data.message || "Failed to remove item!");
                }
                itemToRemove = null;
                document.getElementById('removeConfirmOverlay').style.display = 'none';
            })
            .catch(err => {
                console.error(err);
                alert("Network error while removing item.");
                itemToRemove = null;
                document.getElementById('removeConfirmOverlay').style.display = 'none';
            });
    });

});
