
function getData(obj) {
    var frm = $("<form hidden action='/AdminAttendanceViewer/EmployeeRequests' method='get'> </form>");
    $(frm).append("<input type='hidden' name='EmployeeID' value=" + parseInt($(" #empSelector").val()) + " id='Employee'>");
    $(frm).append("<input type='hidden' name='Month' value=" + parseInt($(" #monthSelector").val()) + " id='Month'>");
    $(frm).append("<input type='hidden' name='Year' value=" + parseInt($(" #yearSelector").val()) + " id='Year'>");
    $(document).find("body").append(frm);
    setTimeout(function () {
        $(frm).submit();
    }, 100);

}

$(document).ready(function () {
    const $actionButtons = $('.action-buttons');
   // const $modal = $('#recordModal');
    const $recordTabs = $('#recordTabs');
    const $recordTabsContent = $('#recordTabsContent');
    const $modalActionButton = $('#modalActionButton');


    $('.contact_list').on('change', 'input[type="checkbox"]', function () {
        
        const checkedBoxes = $('.contact_list input[type="checkbox"]:checked');
        if (checkedBoxes.length > 0) {
            $actionButtons.removeClass('d-none');
        } else {
            $actionButtons.addClass('d-none');
        }
    });


    $('#selectAll').on('change', function () {
        const isChecked = $(this).is(':checked');
        $('.contact_list input[type="checkbox"]').prop('checked', isChecked).trigger('change');
    });


    const $modal = $('#recordModal');
    const $topBar = $('#topBar');
    const $leftTabs = $('#leftTabs');
    const $leftTabsContent = $('#leftTabsContent');
    const $comments = $('#comments');


    $('.action-buttons button').on('click', function () {
        $leftTabs.empty();
        $leftTabsContent.empty();
        $topBar.hide();

        $('.contact_list input[type="checkbox"]:checked').each(function (index) {
            const $row = $(this).closest('tr');
            const empCode = $row.find('td:nth-child(2)').text();
            const name = $row.find('td:nth-child(3)').text();
            const requestType = $row.find('td:nth-child(4)').text();
            const requestDateTime = $row.find('td:nth-child(5)').text();
            const reason = $row.find('td:nth-child(6)').text();

            if (index === 0) {
                $('#topEmpCode').text(empCode);
                $('#topEmpName').text(name);
                $topBar.show();
            }

            const tabId = `tab-${index}`;
            $leftTabs.append(`
    <li class="nav-item">
        <a class="nav-link ${index === 0 ? 'active' : ''}" id="${tabId}" data-toggle="tab" href="#${tabId}-content" role="tab">
            ${requestType}
        </a>
    </li>
    `);

            $leftTabsContent.append(`
    <div class="tab-pane fade ${index === 0 ? 'show active' : ''}" id="${tabId}-content" role="tabpanel">
         <p><strong>Employee Name:</strong> ${name}</p>
        <p><strong>Request Date & Time:</strong> ${requestDateTime}</p>
        <p><strong>Reason:</strong> ${reason}</p>
        
        <div id="dynamicContent-${index}">
            <!-- Placeholder for attachments, employee calendar, etc. -->
        </div>
    </div>
    `);

            $(`#dynamicContent-${index}`).append(`
    <div>
        <h6>Attachments</h6>
        <ul>
            <li><a href="#">Attachment 1</a></li>
            <li><a href="#">Attachment 2</a></li>
        </ul>
    </div>
    <div>
        <h6>Employee Calendar</h6>
        <p>Details about calendar events...</p>
    </div>
    `);
        });

        $modal.modal('show');
    });

    $('#approveButton, #holdButton, #rejectButton').on('click', function () {
        const action = $(this).text();
        const comments = $comments.val();
        console.log(`Action: ${action}`);
        console.log(`Comments: ${comments}`);
        // Perform the desired action here...
        $modal.modal('hide');
    });
});

