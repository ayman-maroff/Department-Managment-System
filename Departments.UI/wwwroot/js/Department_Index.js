
  
        $(document).ready(function () {
            $('.department-name').on('click', function (e) {
                e.preventDefault();
                var departmentId = $(this).data('id');
                var subDepartmentsDiv = $('#subs-' + departmentId);
                if (subDepartmentsDiv.html().trim() === '') {
                    // Load sub-departments if they haven't been loaded yet
                    $.ajax({
                        url: '/Departments/GetSubDepartment/' + departmentId,
                        method: 'GET',
                        success: function (data) {
                            subDepartmentsDiv.html(data);
                            subDepartmentsDiv.slideDown();
                        },
                        error: function () {
                            alert('Error loading sub-departments.');
                        }
                    });
                } else {
                    // Toggle the display if they are already loaded
                    subDepartmentsDiv.slideToggle();
                }
            });

        // Add click event for sub-department names
        $(document).on('click', '.sub-department-name', function (e) {
            e.preventDefault();
        var subDepartmentId = $(this).data('id');
        var subSubDepartmentsDiv = $('#subbs-' + subDepartmentId);
        if (subSubDepartmentsDiv.html().trim() === '') {
            // Load sub-sub-departments if they haven't been loaded yet
            $.ajax({
                url: '/Departments/GetSubDepartment/' + subDepartmentId,
                method: 'GET',
                success: function (data) {
                    subSubDepartmentsDiv.html(data);
                    subSubDepartmentsDiv.slideDown();
                },
                error: function () {
                    alert('Error loading sub-sub-departments.');
                }
            });
                } else {
            // Toggle the display if they are already loaded
            subSubDepartmentsDiv.slideToggle();
                }
            });
        });

