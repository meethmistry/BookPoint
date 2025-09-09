const tempView = '@TempData["View"]';
let preloadedFormHtml = null;

$(document).ready(function () {
    // ==========================
    //  Load / Preload Form
    // ==========================
    if (tempView === "Insert") {
        $.get("/Books/BooksForm", function (data) {
            $("#main-content").html(data);
        }).fail(function () {
            alert("Error loading form.");
        });
    } else {
        $.get("/Books/BooksForm", function (data) {
            preloadedFormHtml = data;
        });
    }

    // ==========================
    //  Navigation Buttons
    // ==========================
    $(document).on("click", "#backToBooks", function () {
        $.get("/Books/Index", function (data) {
            $("#main-content").html(data);
        });
    });

    $(document).on("click", "#addBookBtn", function () {
        if (preloadedFormHtml) {
            $("#main-content").html(preloadedFormHtml);
        } else {
            $.get("/Books/BooksForm", function (data) {
                $("#main-content").html(data);
            });
        }
    });

    // ==========================
    //  Edit Button (from list)
    // ==========================
    $(document).on("click", ".editBookBtn", function () {
        const btn = $(this);

        $.get("/Books/BooksForm", function (data) {
            $("#main-content").html(data);

            // Fill form fields
            $("#Id").val(btn.data("id"));
            $("#BookName").val(btn.data("name"));
            $("#AuthorName").val(btn.data("author"));
            $("#CategoryId").val(btn.data("category"));
            $("#Description").val(btn.data("description"));
            $("#PurchasePrice").val(btn.data("purchase"));
            $("#SalesPrice").val(btn.data("sales"));
            $("#Quantity").val(btn.data("quantity"));

            // PDF preview
            if (btn.data("pdf")) {
                $("#existingPdf").show();
                $("#pdfPreviewLink").attr("href", btn.data("pdf"));
            }

            // Cover image preview
            const coverUrl = btn.data("cover");
            if (coverUrl) {
                $("#coverPreview").attr("src", coverUrl).show();
                $("#previewPlaceholder").hide();
                $("#cancelImage").show();
            }
        });
    });

    // ==========================
    //  Edit Button (from modal)
    // ==========================
    $(document).on("click", ".editBookBtnMore", function () {
        const book = $(this).data();

        $.get("/Books/BooksForm", function (data) {
            $("#main-content").html(data);

            $("#Id").val(book.id);
            $("#BookName").val(book.name);
            $("#AuthorName").val(book.author);
            $("#CategoryId").val(book.categoryid);
            $("#Description").val(book.description);
            $("#PurchasePrice").val(book.purchase);
            $("#SalesPrice").val(book.sales);
            $("#Quantity").val(book.quantity);

            if (book.pdf) {
                $("#existingPdf").show();
                $("#pdfPreviewLink").attr("href", book.pdf);
            }

            if (book.cover) {
                $("#coverPreview").attr("src", book.cover).show();
                $("#previewPlaceholder").hide();
                $("#cancelImage").show();
            }
        });
    });

    // ==========================
    //  Delete Confirmation
    // ==========================
    $(document).on("click", ".openDeleteModal", function () {
        let bookId = $(this).data("id");
        let bookName = $(this).data("name");

        $("#bookToDelete").text(bookName);
        $("#confirmDeleteFinal").attr("data-id", bookId);

        $("#confirmOverlay").css("display", "flex");
    });

    $("#confirmDeleteFinal").on("click", function () {
        let bookId = $(this).data("id");
        $("#deleteBookId").val(bookId);
        $("#deleteBookForm").submit();
        $("#confirmOverlay").hide();
    });

    $("#cancelDelete").on("click", function () {
        $("#confirmOverlay").hide();
    });

    // ==========================
    //  More Details Modal
    // ==========================
    $(document).on("click", ".more-details-btn", function (e) {
        e.preventDefault();

        const bookData = {
            id: $(this).data("id") || "N/A",
            name: $(this).data("name") || "N/A",
            author: $(this).data("author") || "N/A",
            categoryid: $(this).data("categoryid") || "N/A",
            categoryName: $(this).data("categoryname") || "N/A",
            description: $(this).data("description") || "No description available.",
            purchase: parseFloat($(this).data("purchase")) || 0,
            sales: parseFloat($(this).data("sales")) || 0,
            profit: parseFloat($(this).data("profit")) || 0,
            quantity: $(this).data("quantity") || "0",
            created: $(this).data("created") || "-",
            updated: $(this).data("updated") || "-",
            cover: $(this).data("cover") || "/images/no-image.png",
            pdf: $(this).data("pdf") || ""
        };

        $(".editBookBtnMore").data(bookData);

        // Fill modal fields
        $("#modalCoverImage").attr("src", bookData.cover);
        $("#modalBookName").text(bookData.name);
        $("#modalAuthorName").text(bookData.author);
        $("#modalCategoryName").text(bookData.categoryName);
        $("#modalDescription").text(bookData.description);
        $("#modalPurchasePrice").text(bookData.purchase.toFixed(2));
        $("#modalSalesPrice").text(bookData.sales.toFixed(2));
        $("#modalProfit").text(bookData.profit.toFixed(2));
        $("#modalQuantity").text(bookData.quantity);
        $("#modalCreatedAt").text(bookData.created);
        $("#modalLastUpdated").text(bookData.updated);

        if (bookData.pdf) {
            $("#modalPdfWrapper").show();
            $("#modalPdfLink").attr("href", bookData.pdf);
        } else {
            $("#modalPdfWrapper").hide();
        }

        $("#deleteBookBtn").data("id", bookData.id);
        $("#deleteBookBtn").data("name", bookData.name);

        showModal();
    });

    // Close modal when clicking outside
    $(document).on("click", "#bookDetailsModal", function (e) {
        if (e.target === this) closeModal();
    });

    $(document).keydown(function (e) {
        if (e.key === "Escape") closeModal();
    });

    $("#deleteBookBtn").on("click", function (e) {
        e.preventDefault();
        const bookName = $(this).data("name");
        const bookId = $(this).data("id");

        $("#bookToDelete").text(bookName);
        $("#confirmDeleteFinal").attr("data-id", bookId);
        $("#confirmOverlay").css("display", "flex");
    });
});

// ==========================
//  Modal Helpers
// ==========================
function showModal() {
    $("#bookDetailsModal").addClass("show").css("display", "block");
}
function closeModal() {
    $("#bookDetailsModal").removeClass("show").css("display", "none");
}
