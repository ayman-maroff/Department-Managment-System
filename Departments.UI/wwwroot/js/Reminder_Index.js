document.getElementById('DepartmentTime').addEventListener('input', function () {
    const selectedTime = new Date(this.value);
    const currentTime = new Date();

    // If the selected time is in the past, show an error and reset the value
    if (selectedTime < currentTime) {
        alert("Please select a future date and time.");
        this.value = '';  // Reset the invalid value
    }
});
function showTopDepartments() {

    document.getElementById('topDepartments').style.display = 'block';
    document.getElementById('subDepartments').style.display = 'none';
}

function showSubDepartments() {

    document.getElementById('subDepartments').style.display = 'block';
    document.getElementById('topDepartments').style.display = 'none';
}

// JavaScript to Toggle Select All / Remove All Checkboxes
function toggleSelection(departmentType, buttonId) {
    const checkboxes = document.querySelectorAll(`#${departmentType} input[type="checkbox"]`);
    const selectAllButton = document.getElementById(buttonId);
    const allChecked = Array.from(checkboxes).every(checkbox => checkbox.checked); // Check if all are checked
    const anyChecked = Array.from(checkboxes).some(checkbox => checkbox.checked);  // Check if any are checked
    if (allChecked) {
        // If all are selected, uncheck all and change button text to "Select All"
        checkboxes.forEach(checkbox => checkbox.checked = false);
        selectAllButton.innerText = `Select All`;
    } else if (anyChecked) {
        checkboxes.forEach(checkbox => checkbox.checked = false);
        selectAllButton.innerText = `Select All`;
    }
    else {
        // If not all are selected, check all and change button text to "Remove All"
        checkboxes.forEach(checkbox => checkbox.checked = true);
        selectAllButton.innerText = `Remove All`;
    }
}

// Function to update the button state based on current checkbox state
function updateButtonState(departmentType, buttonId) {
    const checkboxes = document.querySelectorAll(`#${departmentType} input[type="checkbox"]`);
    const selectAllButton = document.getElementById(buttonId);
    const allChecked = Array.from(checkboxes).every(checkbox => checkbox.checked); // Check if all are checked
    const anyChecked = Array.from(checkboxes).some(checkbox => checkbox.checked);  // Check if any are checked

    if (allChecked) {
        selectAllButton.innerText = `Remove All`;
    } else if (anyChecked) {
        selectAllButton.innerText = `Remove All`;
    } else {
        selectAllButton.innerText = `Select All`;
    }
}