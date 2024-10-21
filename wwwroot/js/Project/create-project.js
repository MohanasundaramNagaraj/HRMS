document.addEventListener('DOMContentLoaded', async function () {
	
    function FetchAndRenderClientNames() {

        $.ajax({
            url: '/client/getallclientnames',
            type: 'GET',
            success: function (response) {
	
                if (response != null && response.length > 0) {

                    response.forEach(function (name) {
                        let option = new Option(name, name);
                        $('#project_client_select').append(option);
                    });

                    $('#project_client_select').select2();
                    $('project_client_select').css('height', '44px');
                }
            },
            error: function (error) {
                console.error('Error fetching client names:', error);
            }
        });
    }

	await FetchAndRenderClientNames();

	flatePickerDateTime('project_planned_start_date');
	flatePickerDateTime('project_planned_end_date');
   
});


document.getElementById('create-project-form').addEventListener('submit', function (event) {

	event.preventDefault();
	
	let isValidationSuccess = true;

	let projectName = $('#project_name').val();
	let nameErrorMessageElementId = $('#project_name').attr('error-message-element');
	if (projectName == '' || projectName == undefined || projectName == null) {

		$('#' + nameErrorMessageElementId).show();

		isValidationSuccess = false;
	}
	else {
		$('#' + nameErrorMessageElementId).hide();
	}

	let projectClientName = $('#project_client_select').find(":selected").val();
	let projectClientNameErrorMessageElementId = $('#project_client_select').attr('error-message-element');

	if (projectClientName == '' || projectClientName == null || projectClientName == undefined) {

		$('#' + projectClientNameErrorMessageElementId).show();

		isValidationSuccess = false;
	}
	else {
		$('#' + projectClientNameErrorMessageElementId).hide();
	}


	let projectDescription = $('#project_description').val();
	let descriptionErrorMessageElementId = $('#project_description').attr('error-message-element');
	if (projectDescription == '' || projectDescription == undefined || projectDescription == null) {

		$('#' + descriptionErrorMessageElementId).show();

		isValidationSuccess = false;
	}
	else {
		$('#' + descriptionErrorMessageElementId).hide();
	}

	

	const plannedStartDateInputValue = document.getElementById('project_planned_start_date').value;

	const projectStartErrorMessageElementId = $('#project_planned_start_date').attr('error-message-element');
	if (plannedStartDateInputValue == '' || plannedStartDateInputValue == undefined || plannedStartDateInputValue == null) {

		$('#' + projectStartErrorMessageElementId).text('project start date required').show();

		isValidationSuccess = false;
	}
	else {
		$('#' + projectStartErrorMessageElementId).text('').hide();
	}


	const plannedEndDateInputValue = document.getElementById('project_planned_end_date').value;

	const projectEndErrorMessageElementId = $('#project_planned_end_date').attr('error-message-element');

	if (plannedEndDateInputValue == '' || plannedEndDateInputValue == undefined || plannedEndDateInputValue == null) {

		$('#' + projectEndErrorMessageElementId).text('project end date required').show();

		isValidationSuccess = false;
	}
	else {
		$('#' + projectEndErrorMessageElementId).text('').hide();
	}

	if (
		plannedStartDateInputValue != '' && plannedStartDateInputValue != undefined && plannedStartDateInputValue != null
		&&
		plannedEndDateInputValue != '' && plannedEndDateInputValue != undefined && plannedEndDateInputValue != null
	) {

		const projectStartDate = new Date(plannedStartDateInputValue);
		console.log(projectStartDate);
		const projectEndDate = new Date(plannedEndDateInputValue);
		console.log(projectEndDate);
		const currentDate = new Date();
		console.log(currentDate);

		if (projectStartDate <= currentDate) {

			$('#' + projectStartErrorMessageElementId).text('Planned start date should be in the future.').show();
			isValidationSuccess = false;

		} else {
			$('#' + projectStartErrorMessageElementId).text('').hide();
		}

		if (projectEndDate <= currentDate || projectEndDate <= projectStartDate) {

			$('#' + projectEndErrorMessageElementId).text('Planned end date should be later than both the current time and the planned start date.').show();
			isValidationSuccess = false;

		} else {
			$('#' + projectEndErrorMessageElementId).text('').hide();
		}
	}

	if (isValidationSuccess) {
		this.submit();  
	}
	
});

