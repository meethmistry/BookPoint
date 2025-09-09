$(document).ready(function () {
    var categoryIdToDelete = null;

    $("#categoryForm").submit(function (e) {
        e.preventDefault();

        $.ajax({
            url: $(this).attr("action"),
            type: $(this).attr("method"),
            data: $(this).serialize(),
            success: function (response) {
                if (response.success) {
                    $("#error-summary-category").remove();

                    $.get("/Category/Categories", function (data) {
                        $(".page-content").html(data);
                    });
                } else {
                    if ($("#error-summary-category").length) {
                        $("#error-summary-category").text(response.message);
                    } else {
                        $("#categoryForm").prepend(
                            `<div class="alert alert-danger" id="error-summary-category">${response.message}</div>`
                        );
                    }
                }
            },
            error: function () {
                alert("Something went wrong! Try again.");
            }
        });
    });

    $(document).on("click", ".edit-btn", function () {
        var categoryId = $(this).data("id");
        var categoryName = $(this).data("name");

        $("#CategoryId").val(categoryId);
        $("#Name").val(categoryName);
        $("#submitBtn").text("Update Category");
    });

    $("#Name").on("input", function () {
        if ($(this).val().trim() === "") {
            $("#CategoryId").val("");
            $("#submitBtn").text("Add Category");
        }
    });

    $(document).on("click", ".delete-btn", function () {
        categoryIdToDelete = $(this).data("id");
        var categoryName = $(this).data("name");

        $("#categoryToDelete").text(categoryName);
        $("#confirmationOverlay").fadeIn(200);
    });

    $("#cancelDelete").click(function () {
        $("#confirmationOverlay").fadeOut(200);
        categoryIdToDelete = null;
    });

    $("#confirmationOverlay").click(function (e) {
        if (e.target === this) {
            $("#confirmationOverlay").fadeOut(200);
            categoryIdToDelete = null;
        }
    });

    $(document).keydown(function (e) {
        if (e.keyCode === 27) {
            $("#confirmationOverlay").fadeOut(200);
            categoryIdToDelete = null;
        }
    });

    $("#confirmDelete").click(function () {
        if (categoryIdToDelete) {
            $.ajax({
                url: $("#deleteForm").attr("action"),
                type: "POST",
                data: {
                    id: categoryIdToDelete,
                    __RequestVerificationToken: $("#deleteForm input[name='__RequestVerificationToken']").val()
                },
                success: function (response) {
                    if (response.success) {
                        $("#categoriesTable tbody tr").filter(function () {
                            return $(this).find(".delete-btn").data("id") == categoryIdToDelete;
                        }).remove();

                        $("#categoriesTable tbody tr").each(function (i) {
                            $(this).find("td:first").text(i + 1);
                        });

                        $("#confirmationOverlay").fadeOut(200);
                        categoryIdToDelete = null;
                    } else {
                        alert(response.message || "Error deleting category.");
                    }
                },
                error: function () {
                    alert("Something went wrong while deleting.");
                }
            });
        }
    });

    $("#searchCategory").on("keyup", function () {
        var value = $(this).val().toLowerCase();
        $("#categoriesTable tbody tr").filter(function () {
            $(this).toggle($(this).find(".category-name").text().toLowerCase().indexOf(value) > -1);
        });
    });
});
