
$(document).ready(function () {

    debugger;
    var selectedEmployeeId = null;

    $(document).on('change', '.row-checkbox', function () {

        // Uncheck all other checkboxes
        $('.row-checkbox').not(this).prop('checked', false);

        if ($(this).is(':checked')) {
            selectedEmployeeId = $(this).data('id') || null;
            $('#hdnEmployeeId').val(selectedEmployeeId);
        } else {
            selectedEmployeeId = null;
            $('#hdnEmployeeId').val('');
        }
    });

    $("#addButton").click(function () {
        debugger;
        OpenEmployeeCreate("ADD", 0);
    });

    $("#editButton").click(function () {
        debugger;
  

        if (selectedEmployeeId) {
            $('#hdnEmployeeId').val(selectedEmployeeId);
            OpenEmployeeCreate("EDIT", selectedEmployeeId);
        } else {
            alert("Please select an employee first.");
        }
    });

    $("#viewButton").click(function () {
        debugger;

        if (selectedEmployeeId) {
            $('#hdnEmployeeId').val(selectedEmployeeId);
            OpenEmployeeCreate("VIEW", selectedEmployeeId);
        } else {
            alert("Please select an employee first.");
        }
    });

    $("#btnUpdate").click(function () {
        debugger;
        updateEmployeeDetails();
    });

    $("#btnSave").click(function () {
        debugger;
        saveEmployee();
    });


    $('#sameAddress').on('change', function () {
        debugger;
        var cAddress1 = $('#cAddress1').val().trim();
        var cCity = $('#cCity').val().trim();
        var cPincode = $('#cPincode').val().trim();
        var cState = $('#cState').val();
        var cCountry = $('#cCountry').val();

        if ($(this).is(':checked')) {

            if (cAddress1 === '' || cCity === '' || cPincode === '' || cState === '' || cCountry === '') {
                alert('Please fill Current Address first.');
                $(this).prop('checked', false);
                return;
            }

            // Copy all fields
            $('#pAddress1').val(cAddress1);
            $('#pAddress2').val($('#cAddress2').val());
            $('#pCity').val(cCity);
            $('#pPincode').val(cPincode);
            $('#pState').val(cState).trigger('change');     
            $('#pCountry').val(cCountry).trigger('change');  

        } else {

            $('#pAddress1').val('');
            $('#pAddress2').val('');
            $('#pCity').val('');
            $('#pPincode').val('');
            $('#pState').val('').trigger('change');
            $('#pCountry').val('').trigger('change');
        }
    });

    $("#deleteButton").click(function () {
        debugger;
        if (selectedEmployeeId) {

            if (confirm("Are you sure you want to delete this employee?")) {
                deleteEmployee(selectedEmployeeId);
            }

        } else {
            alert("Please select an employee first.");
        }

    });


});

function OpenEmployeeCreate(fMode, entryId) {
    debugger;

    var frm = $("<form>", {
        action: "/Employee/Maintanance",
        method: "post"
    });

    frm.append(`<input type="hidden" name="FMode" value="${fMode}" />`);
    frm.append(`<input type="hidden" name="EntryID" value="${entryId}" />`);

    $("body").append(frm);
    frm.submit();
}



function calculateAge(dob) {
    let birthDate = new Date(dob);
    let today = new Date();

    let age = today.getFullYear() - birthDate.getFullYear();
    let monthDiff = today.getMonth() - birthDate.getMonth();

    if (monthDiff < 0 ||
        (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
        age--;
    }

    return age;
}

$('#date3').on('change', function () {
    debugger;
    let dobValue = $(this).val();

    if (dobValue) {
        let age = calculateAge(dobValue);
        $('#age').val(age);
    } else {
        $('#age').val('');
    }
});
function saveEmployee() {
    debugger;
    var formData = new FormData();
    formData.append("EmployeeCode", $("input[name='EmployeeCode']").val());
    formData.append("Designation", $("#Designation").val());
    formData.append("EmployeeName", $("input[name='EmployeeName']").val());
    formData.append("Gender", $(".gender-select").val());
    formData.append("DOB", $(".datetimepicker").val());

    formData.append("BloodGroup", $(".blood-group").val());
    formData.append("Nationality", $(".nationality").val());
    formData.append("MartialStatus", $(".marital-status").val());
    formData.append("ReportingHeadMailID", $("#ReportingHeadMailID").val());
    formData.append("DateOfJoining", $("#date3").val());

   
    formData.append("PhoneNumber", $("input[name='PhoneNumber']").val());
    formData.append("AlternateMoblieNumber", $("#AlternateMoblieNumber").val());
    formData.append("Email", $("input[name='Email']").val());

    
    formData.append("EmergencyContactName", $("#EmergencyContactName").val());
    formData.append("EmergencyContactRelation", $("#emgRelation").val());
    formData.append("EmergencyContactNumber", $("#EmergencyContactNumber").val());

    formData.append("EmergencyAlternateNumber", $("#EmergencyAlternateNumber").val());


    formData.append("FatherName", $("#FatherName").val());
    formData.append("MotherName", $("#MotherName").val());
    formData.append("SpouseName", $("#SpouseName").val());
    formData.append("NumberOfDependents", $("#NumberOfDependents").val());

   
    formData.append("PanNumber", $("#PanNumber").val());
    formData.append("AadhaarNumber", $("#AadhaarNumber").val());
    formData.append("PassportNumber", $("#PassportNumber").val());
    formData.append("DrivingLicenseNumber", $("#DrivingLicenseNumber").val());
    debugger;
    var isSame = $("#sameAddress").is(":checked");
    debugger;
    // Flag (important for backend)
    formData.append("IsSameAsCurrent", isSame);

    // ================= CURRENT ADDRESS =================
    formData.append("CurrentAddress.Address1", $("#cAddress1").val());
    formData.append("CurrentAddress.Address2", $("#cAddress2").val());
    formData.append("CurrentAddress.City", $("#cCity").val());
    formData.append("CurrentAddress.StateID", $("#cState").val());
    formData.append("CurrentAddress.Country", $("#cCountry").val());
    formData.append("CurrentAddress.Pincode", $("#cPincode").val());

    // ================= PERMANENT ADDRESS =================
    if (isSame) {
        // Copy current → permanent
        formData.append("PermanentAddress.Address1", $("#cAddress1").val());
        formData.append("PermanentAddress.Address2", $("#cAddress2").val());
        formData.append("PermanentAddress.City", $("#cCity").val());
        formData.append("PermanentAddress.StateID", $("#cState").val());
        formData.append("PermanentAddress.Country", $("#cCountry").val());
        formData.append("PermanentAddress.Pincode", $("#cPincode").val());
    } else {
        // Separate permanent
        formData.append("PermanentAddress.Address1", $("#pAddress1").val());
        formData.append("PermanentAddress.Address2", $("#pAddress2").val());
        formData.append("PermanentAddress.City", $("#pCity").val());
        formData.append("PermanentAddress.StateID", $("#pState").val());
        formData.append("PermanentAddress.Country", $("#pCountry").val());
        formData.append("PermanentAddress.Pincode", $("#pPincode").val());
    }



  
    var photo = $("input[type='file']")[0].files[0];
    if (photo) {
        formData.append("Photo", photo);
    }

    $.ajax({
        url: '/Employee/SaveEmployeeDetails',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (res) {
            debugger;
            if (res.success) {
                alert("Employee saved successfully ✅");
               
            } else {
                alert(res.message);
            }
        },
        error: function () {
            alert("Something went wrong ❌");
        }
    });
}
function updateEmployeeDetails() {
    debugger;
    var formData = new FormData();

    
    formData.append("EmployeeId", $("#hdnEmployeeId").val());
    formData.append("EmployeeName", $("input[name='EmployeeName']").val());
    formData.append("Gender", $("#Gender").val());
    formData.append("DOB", $("#DOB").val());
    formData.append("BloodGroup", $(".blood-group").val());
    formData.append("Nationality", $("#Nationality").val());
    formData.append("MartialStatus", $("#MartialStatus").val());
    formData.append("PhoneNumber", $("#PhoneNumber").val());
    formData.append("Email", $("#Email").val());
    formData.append("FatherName", $("#FatherName").val());
    formData.append("MotherName", $("#MotherName").val());
    formData.append("SpouseName", $("#SpouseName").val());
    formData.append("AadhaarNumber", $("#AadhaarNumber").val());
    formData.append("PanNumber", $("#PanNumber").val());
    formData.append("PassportNumber", $("#PassportNumber").val());
    formData.append("DrivingLicenseNumber", $("#DrivingLicenseNumber").val());
    formData.append("Designation", $("#Designation").val());
    formData.append("DateOfJoining", $("#DateOfJoining").val());
    formData.append("ReportingHeadMailID", $("#ReportingHeadMailID").val());
    formData.append("AlternateMoblieNumber", $("#AlternateMoblieNumber").val());
    formData.append("EmergencyContactName", $("#EmergencyContactName").val());
    formData.append("EmergencyContactNumber", $("#EmergencyContactNumber").val());
    formData.append("EmergencyContactRelation", $("#EmergencyContactRelation").val());
    formData.append("EmergencyAlternateNumber", $("#EmergencyAlternateNumber").val());

 
   

    // ================= CURRENT ADDRESS =================
    formData.append("CurrentAddress.Address1", $("#cAddress1").val());
    formData.append("CurrentAddress.Address2", $("#cAddress2").val());
    formData.append("CurrentAddress.City", $("#cCity").val());
    formData.append("CurrentAddress.StateID", $("#cState").val());
    formData.append("CurrentAddress.Country", $("#cCountry").val());
    formData.append("CurrentAddress.Pincode", $("#cPincode").val());
    formData.append("CurrentAddress.IsPermanentAddress", $("#sameAddress").is(":checked"));

    // ================= PERMANENT ADDRESS =================
    formData.append("PermanentAddress.Address1", $("#pAddress1").val());
    formData.append("PermanentAddress.Address2", $("#pAddress2").val());
    formData.append("PermanentAddress.City", $("#pCity").val());
    formData.append("PermanentAddress.StateID", $("#pState").val());
    formData.append("PermanentAddress.Country", $("#pCountry").val());
    formData.append("PermanentAddress.Pincode", $("#pPincode").val());


    debugger;
    var photoInput = document.getElementById("Photo");

    if (photoInput && photoInput.files.length > 0) {
        formData.append("Photo", photoInput.files[0]);
    }



    $.ajax({
        url: '/Employee/UpdateEmployeeDetails',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        beforeSend: function () {
            $("#btnUpdate").prop("disabled", true).text("Updating...");
        },
        success: function (res) {
            if (res.success) {
                alert(res.message);
            } else {
                alert(res.message);
            }
        },
        error: function () {
            alert("Something went wrong. Please try again.");
        },
        complete: function () {
            $("#btnUpdate").prop("disabled", false).text("Update");
        }
    });
}

function deleteEmployee(employeeId) {
    debugger;
    $.ajax({
        url: '/Employee/Delete',   // Change if your controller name is different
        type: 'POST',
        data: { id: employeeId },
        success: function (response) {

            if (response.success) {
                alert("Employee deleted successfully.");
                location.reload(); // refresh page after delete
            } else {
                alert(response.message);
            }

        },
        error: function () {
            alert("Error deleting employee.");
        }
    });

}
