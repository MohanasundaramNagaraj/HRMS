$(document).ready(function () {
    $('.page-loader-wrapper').hide();
});

const AttendanceStatus = {
    Present: 0,
     HalfDay: 1,
    Absent: 2,
    Weekend: 3,
    Holiday: 4,
    PermissionNeeded: 5,
    PendingCheckOut: 6
};


async function FetchCountryCode(select2ElementId) {

	fetch('https://restcountries.com/v3.1/all')
		.then(response => response.json())
		.then(data => {
			data.sort((a, b) => a.name.common.localeCompare(b.name.common));
			data.forEach(country => {
				if (country.idd && country.idd.root) {
					let countryCode = country.idd.root + (country.idd.suffixes ? country.idd.suffixes[0] : "");
					let option = new Option(` ${country.name.common} (${countryCode})`, countryCode);

					$('#' + select2ElementId).append(option);
				}
			});

		})
		.catch(error => {
			console.error('Error fetching country codes:', error);
		});
}

function flatePickerDateTime(elementId) {
	
	flatpickr("#" + elementId, {
		enableTime: true,
		dateFormat: "Y-m-d H:i",
	});
}