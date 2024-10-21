

document.addEventListener('DOMContentLoaded', async function () {
	
	await FetchCountryCode('client_country_phone_code');

	$('.select2-container .select2-selection--single').css('height', '44px');
	$('#client_country_phone_code').select2();
});

document.getElementById('client-create-form').addEventListener('submit', function (event) {

	
	let isValidationSuccess = true;

	let clientName = $('#client_name').val();
	if (clientName == '' || clientName == undefined || clientName == null) {

		let nameErrorMessageElementId = $('#client_name').attr('error-message-element');

		$('#' + nameErrorMessageElementId).show();

		isValidationSuccess = false;
	}

	let clientEmail = $('#client_email').val();
	if (clientEmail == '' || clientEmail == undefined || clientEmail == null) {
		let emailErrorMessageElementId = $('#client_email').attr('error-message-element');

		$('#' + emailErrorMessageElementId).show();

		isValidationSuccess = false;
	}

	let clientAddress = $('#client_address').val();
	if (clientAddress == '' || clientAddress == undefined || clientAddress == null) {

		let addressErrorMessageElementId = $('#client_address').attr('error-message-element');

		$('#' + addressErrorMessageElementId).show();

		isValidationSuccess = false;
	}

	let clientDescription = $('#client_description').val();
	if (clientDescription == '' || clientDescription == undefined || clientDescription == null) {

		let descriptionErrorMessageElementId = $('#client_description').attr('error-message-element');

		$('#' + descriptionErrorMessageElementId).show();

		isValidationSuccess = false;
	}

	let clientContryCode = $('#client_country_phone_code').find(":selected").val();
	let clientPhoneNumber = $('#client_phone_number').val();

	if (clientContryCode == undefined || clientContryCode == null || clientPhoneNumber == undefined || clientPhoneNumber == null) {

		console.log(`Country code value : ${clientContryCode} Phone Number value : ${clientPhoneNumber}`)
		return;
	}

	if (clientContryCode != '' && clientPhoneNumber == '') {
		let phoneNumberErrorMessageElementId = $('#client_phone_number').attr('error-message-element');

		$('#' + phoneNumberErrorMessageElementId).show();

		isValidationSuccess = false;
	}

	if (clientContryCode == '' && clientPhoneNumber != '') {
		let contryCodeErrorMessageElementId = $('#client_country_phone_code').attr('error-message-element');

		$('#' + contryCodeErrorMessageElementId).show();

		isValidationSuccess = false;
	}

	if (!isValidationSuccess) {
		event.preventDefault();
	}
});