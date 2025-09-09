$(document).ready(function () {
    let currentAction = null;
    let agentId = null;

    // Form submission with AJAX
    $("#agentForm").submit(function (e) {
        e.preventDefault();

        $.ajax({
            url: $(this).attr("action"),
            type: $(this).attr("method"),
            data: $(this).serialize(),
            success: function (response) {
                if (response.success) {
                    // Remove any existing error messages
                    $("#error-summary-agent").remove();

                    // Refresh the entire page content
                    $.get("/Agent/DeliveryAgents", function (data) {
                        $(".page-content").html(data);
                    });

                    // Reset form
                    resetForm();
                } else {
                    // Show error message
                    if ($("#error-summary-agent").length) {
                        $("#error-summary-agent").text(response.message);
                    } else {
                        $("#agentForm").prepend(
                            `<div class="alert alert-danger" id="error-summary-agent">${response.message}</div>`
                        );
                    }
                }
            },
            error: function () {
                alert("Something went wrong! Try again.");
            }
        });
    });

    // Edit button click
    $(document).on("click", ".edit-btn", function () {
        var id = $(this).data("id");
        var username = $(this).data("username");
        var email = $(this).data("email");
        var phone = $(this).data("phone");
        var password = $(this).data("password");

        // Fill form fields
        $("#AgentId").val(id);
        $("#Agent_UserName").val(username);
        $("#User_Email").val(email);
        $("#Agent_Phone").val(phone);
        $("#Password").val(password);
        $("#ConfirmPassword").val(password);

        // Change button text
        $("#submitBtn").text("Update Agent");

        // Remove any error messages when editing
        $("#error-summary-agent").remove();
    });

    // Reset form function
    function resetForm() {
        $("#agentForm")[0].reset();
        $("#AgentId").val("");
        $("#submitBtn").text("Save Agent");
        $("#error-summary-agent").remove();
    }

    // Reset button click
    $("#resetBtn").click(function () {
        resetForm();
    });

    // Clear form when any input changes (to reset from edit mode)
    $("#Agent_UserName, #User_Email, #Agent_Phone, #Password, #ConfirmPassword").on("input", function () {
        var allEmpty = $("#Agent_UserName").val().trim() === "" &&
            $("#User_Email").val().trim() === "" &&
            $("#Agent_Phone").val().trim() === "" &&
            $("#Password").val().trim() === "" &&
            $("#ConfirmPassword").val().trim() === "";

        if (allEmpty) {
            $("#AgentId").val("");
            $("#submitBtn").text("Save Agent");
        }
    });

    // Function to show overlay for any action
    function showConfirmation(action, id, name, isActive) {
        currentAction = action;
        agentId = id;

        // Set header color, icon, title, message, and confirm button
        if (action === "delete") {
            $("#confirmationHeader").removeClass().addClass("card-header bg-danger text-white");
            $("#confirmationIcon").removeClass().addClass("fas fa-exclamation-triangle");
            $("#confirmationTitle").text("Confirm Deletion");
            $("#confirmationMessage").html(`Are you sure you want to delete agent "<strong>${name}</strong>"?`);
            $("#confirmationNote").text("This action cannot be undone.");
            $("#confirmAction").removeClass().addClass("btn btn-danger").text("Delete");
        } else if (action === "activate") {
            $("#confirmationHeader").removeClass().addClass("card-header bg-success text-white");
            $("#confirmationIcon").removeClass().addClass("fas fa-user-check");
            $("#confirmationTitle").text("Confirm Activation");
            $("#confirmationMessage").html(`Are you sure you want to activate agent "<strong>${name}</strong>"?`);
            $("#confirmationNote").text("The user will regain access immediately.");
            $("#confirmAction").removeClass().addClass("btn btn-success").text("Activate");
        } else if (action === "deactivate") {
            $("#confirmationHeader").removeClass().addClass("card-header bg-warning text-white");
            $("#confirmationIcon").removeClass().addClass("fas fa-user-slash");
            $("#confirmationTitle").text("Confirm Deactivation");
            $("#confirmationMessage").html(`Are you sure you want to deactivate agent "<strong>${name}</strong>"?`);
            $("#confirmationNote").text("The user will not be able to log in until reactivated.");
            $("#confirmAction").removeClass().addClass("btn btn-warning").text("Deactivate");
        }
        $("#confirmationOverlay").fadeIn(200);
    }

    // Delete button click
    $(document).on("click", ".delete-agent-btn", function () {
        let id = $(this).data("id");
        let name = $(this).data("username");
        showConfirmation("delete", id, name);
    });

    // Active/Inactive toggle button click
    $(document).on("click", ".status-btn", function () {
        let id = $(this).data("id");
        let name = $(this).data("username");
        let isActive = $(this).data("isactive") === true || $(this).data("isactive") === "true";

        if (isActive) {
            showConfirmation("deactivate", id, name, isActive);
        } else {
            showConfirmation("activate", id, name, isActive);
        }
    });

    // Cancel button click
    $("#cancelAction").click(function () {
        $("#confirmationOverlay").fadeOut(200);
        agentId = null;
        currentAction = null;
    });

    // Click outside overlay closes it
    $("#confirmationOverlay").click(function (e) {
        if (e.target === this) {
            $("#confirmationOverlay").fadeOut(200);
            agentId = null;
            currentAction = null;
        }
    });

    // ESC key closes overlay
    $(document).keydown(function (e) {
        if (e.key === "Escape") {
            $("#confirmationOverlay").fadeOut(200);
            agentId = null;
            currentAction = null;
        }
    });

    // Confirm action button
    $("#confirmAction").click(function () {
        if (!agentId || !currentAction) return;

        if (currentAction === "delete") {
            // Delete agent
            $("#deleteId").val(agentId);

            $.ajax({
                url: $("#deleteForm").attr("action"),
                type: "POST",
                data: {
                    id: agentId,
                    __RequestVerificationToken: $("#deleteForm input[name='__RequestVerificationToken']").val()
                },
                success: function (response) {
                    if (response.success) {
                        // Remove the agent card from DOM
                        $(`.delete-agent-btn[data-id="${agentId}"]`).closest(".agent-card-wrapper").fadeOut(300, function () {
                            $(this).remove();
                        });

                        $("#confirmationOverlay").fadeOut(200);
                        agentId = null;
                        currentAction = null;
                    } else {
                        alert(response.message || "Error deleting agent.");
                    }
                },
                error: function () {
                    alert("Something went wrong while deleting.");
                }
            });
        } else if (currentAction === "activate" || currentAction === "deactivate") {
            // Toggle status
            $("#toggleId").val(agentId);

            $.ajax({
                url: $("#toggleStatusForm").attr("action"),
                type: "POST",
                data: {
                    id: agentId,
                    __RequestVerificationToken: $("#toggleStatusForm input[name='__RequestVerificationToken']").val()
                },
                success: function (response) {
                    if (response.success) {
                        // Update the UI elements
                        var agentCard = $(`.status-btn[data-id="${agentId}"]`).closest(".agent-card");
                        var statusBtn = agentCard.find(".status-btn");
                        var statusSpan = agentCard.find(".agent-status span");

                        if (response.isActive) {
                            statusBtn.removeClass("btn-secondary").addClass("btn-success").text("Active");
                            statusBtn.data("isactive", "true");
                            statusSpan.removeClass("text-warning").addClass("text-success").text("Active");
                        } else {
                            statusBtn.removeClass("btn-success").addClass("btn-secondary").text("Inactive");
                            statusBtn.data("isactive", "false");
                            statusSpan.removeClass("text-success").addClass("text-warning").text("Inactive");
                        }

                        $("#confirmationOverlay").fadeOut(200);
                        agentId = null;
                        currentAction = null;

                        // Re-apply filters after status change
                        filterAgents();
                    } else {
                        alert(response.message || "Error updating agent status.");
                    }
                },
                error: function () {
                    alert("Something went wrong while updating status.");
                }
            });
        }
    });
});