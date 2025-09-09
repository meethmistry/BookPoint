$(function () {
    const id = $("#Id").val();
    const title = $("#formTitle");
    const btn = $(".btn-primary");
    if (id && parseInt(id) > 0) {
        title.text("Update Book");
        btn.text("Update Book");

        // Load existing image if available
        loadExistingImage();
    } else {
        title.text("Add New Book");
        btn.text("Save Book");
    }
});

// Load existing image on edit mode
function loadExistingImage() {
    const preview = document.getElementById('coverPreview');
    const placeholder = document.getElementById('previewPlaceholder');
    const cancelBtn = document.getElementById('cancelImage');
    const existingImagePath = preview.getAttribute('data-image');

    if (existingImagePath && existingImagePath.trim() !== '') {
        preview.src = existingImagePath;
        preview.style.display = 'inline'; // Keep it as inline to match the current state
        placeholder.style.display = 'none';
        cancelBtn.style.display = 'block';

        console.log('Loaded existing image:', existingImagePath);
        console.log('Preview display after load:', preview.style.display);
    }
}

// Cover image preview
function previewCover(input) {
    const preview = document.getElementById('coverPreview');
    const placeholder = document.getElementById('previewPlaceholder');
    const cancelBtn = document.getElementById('cancelImage');
    const validationSpan = document.getElementById('coverImageValidation');

    if (input.files && input.files[0]) {
        const reader = new FileReader();
        reader.onload = function (e) {
            preview.src = e.target.result;
            preview.style.display = 'block';
            placeholder.style.display = 'none';
            cancelBtn.style.display = 'block';
            validationSpan.style.display = 'none';
        };
        reader.readAsDataURL(input.files[0]);
    } else {
        removeImage();
    }
}

function removeImage(event) {
    if (event) event.stopPropagation();

    const input = document.getElementById('coverImage');
    const preview = document.getElementById('coverPreview');
    const placeholder = document.getElementById('previewPlaceholder');
    const cancelBtn = document.getElementById('cancelImage');
    const validationSpan = document.getElementById('coverImageValidation');

    input.value = '';
    preview.src = '';
    preview.style.display = 'none';
    placeholder.style.display = 'block';
    cancelBtn.style.display = 'none';

    console.log('Image removed - preview display:', preview.style.display);
}

// Updated validation function
function validateForm() {
    const fileInput = document.getElementById('coverImage');
    const validationSpan = document.getElementById('coverImageValidation');
    const preview = document.getElementById('coverPreview');
    const isEditMode = $("#Id").val() && parseInt($("#Id").val()) > 0;

    // Check if either a new file is selected OR we have a visible existing image
    const hasNewFile = fileInput.files && fileInput.files.length > 0;
    // Fixed: Check if image is visible (not hidden) AND has a source
    const hasVisibleImage = preview.style.display !== 'none' && preview.src !== '' && preview.src !== window.location.href;

    console.log('Validation Debug:');
    console.log('- Is Edit Mode:', isEditMode);
    console.log('- Has New File:', hasNewFile);
    console.log('- Has Visible Image:', hasVisibleImage);
    console.log('- Preview Display:', preview.style.display);
    console.log('- Preview Src:', preview.src);

    // In add mode: require new file
    // In edit mode: require either new file OR visible existing image
    if (!hasNewFile && (!isEditMode || !hasVisibleImage)) {
        validationSpan.style.display = 'block';
        validationSpan.scrollIntoView({ behavior: 'smooth', block: 'center' });
        return false;
    }

    validationSpan.style.display = 'none';
    return true;
}

// Clear entire form
function clearForm() {
    const form = document.querySelector('form');
    form.reset();
    removeImage();
    $(form).find('.text-danger').text('');
    $(form).find('.is-invalid').removeClass('is-invalid');

    // Reload existing image if in edit mode
    if ($("#Id").val() && parseInt($("#Id").val()) > 0) {
        loadExistingImage();
    }
}

$(document).ready(function () {
    const form = $('form');
    form.removeData("validator");
    form.removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse(form);
});