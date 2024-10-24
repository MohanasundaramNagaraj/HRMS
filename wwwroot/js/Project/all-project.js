
document.addEventListener('DOMContentLoaded', function () {

    let pageDataCountSelectElement = $('#project_page_count');
    $(pageDataCountSelectElement).select2();
    $(pageDataCountSelectElement).on('change', function () {
        FetchProject();
    });
    
    FetchProject();
});


function FetchProject() {
    
    let page_data_count = parseInt($('#project_page_count').val());
    let searchText = $('#project_dt_search').val();
    let pageNumber = parseInt($('#project-pagination-container li.active a.page-link').attr('data-page'));
    
    if (isNaN(pageNumber) || pageNumber == undefined || pageNumber == null || pageNumber == '') {
        pageNumber = 1;
    }

    loadProjects(pageNumber, page_data_count, searchText);
}


function loadProjects(pageNumber, page_data_count, searchText) {

    $.ajax({
        url: `/project/getallprojects?PageNumber=${pageNumber}&PageSize=${page_data_count}&SearchText=${searchText}`,
        type: 'GET',
        success: function (response) {

            renderProjects(response.Projects);

            renderPagination(response.TotalNumberOfPage, response.CurrentPageNumber, response.PreviousPageNumber, response.NextPageNumber);
        },
        error: function (error) {
            console.error('Error fetching clients:', error);
        }
    });
}

function renderProjects(projects) {

    const clientContainer = document.getElementById('project-list-container');
    clientContainer.innerHTML = '';

    projects.forEach(project => {

        let memberHtmlString = ''
        
        if (project.Members.length > 0) {

            let Options = '';

            $(project.Members).each(function (index, value) {

                Options += `<option value="${value.Id}">${value.Name}</option>`;
            });

            memberHtmlString = `<select name="ProjectMember" class="form-select form-select-sm" id="project_member_${project.ProjectId}">
										${Options}
								</select>`
        }
        else {
            memberHtmlString = `<a href="/project/addmembers" class="btn btn-primary">Add Members</a>`
        }

        clientContainer.innerHTML += `
               <tr class="odd gradeX">
					<td class="center sorting_1">${project.ProjectName}</td>
					<td class="center">${project.ClientName}</td>
					<td class="center">${project.PlannedEndDate}</td>

					<td class="center">
						${memberHtmlString}
					</td>

					<td class="center">
						<div class="badge col-green">${project.Status}</div>
					</td>
					<td class="center dt-type-numeric">
						<div class="progress-xs not-rounded progress">
						     <div class="progress-bar progress-bar-warning" role="progressbar" aria-valuenow="${project.ProgressPercentage}" aria-valuemin="0" aria-valuemax="100" style="width: ${project.ProgressPercentage}%">
							      <span class="sr-only">${project.ProgressPercentage}%</span>
						     </div>
					    </div>
					</td>
					<td class="center">
						<a href="/project/getprojectdetail?ProjectId=${project.ProjectId}" class="btn btn-tbl-edit">
							<i class="material-icons">create</i>
						</a>
						<a href="" class="btn btn-tbl-delete" onClick="DeleteProject(${project.ProjectId})">
							<i class="material-icons">delete_forever</i>
						</a>
					</td>
				</tr>
            `;

        $(`#project_member_${project.ProjectId}`).select2();
    });
}

function renderPagination(totalPages, currentPage, PreviousPageNumber, NextPageNumber) {
    const paginationContainer = document.getElementById('project-pagination-container');
    paginationContainer.innerHTML = '';

    paginationContainer.innerHTML += `<li class="page-item ${currentPage === 1 ? 'disabled' : ''}">
            <a href="#" class="page-link" data-page="1">«</a></li>`;

    paginationContainer.innerHTML += `<li class="page-item ${currentPage === 1 ? 'disabled' : ''}">
            <a href="#" class="page-link" data-page="${PreviousPageNumber}">‹</a></li>`;

    if (PreviousPageNumber < currentPage) {
        paginationContainer.innerHTML += `<li class="page-item ${PreviousPageNumber}">
                <a href="#" class="page-link" data-page="${PreviousPageNumber}">${PreviousPageNumber}</a></li>`;
    }

    paginationContainer.innerHTML += `<li class="page-item active">
                <a href="#" class="page-link" data-page="${currentPage}">${currentPage}</a></li>`;

    if (NextPageNumber > currentPage) {
        paginationContainer.innerHTML += `<li class="page-item ${NextPageNumber}">
                <a href="#" class="page-link" data-page="${NextPageNumber}">${NextPageNumber}</a></li>`;
    }

    paginationContainer.innerHTML += `<li class="page-item ${currentPage === totalPages ? 'disabled' : ''}">
            <a href="#" class="page-link" data-page="${NextPageNumber}">›</a></li>`;

    paginationContainer.innerHTML += `<li class="page-item ${currentPage === totalPages ? 'disabled' : ''}">
            <a href="#" class="page-link" data-page="${totalPages}">»</a></li>`;

    document.querySelectorAll('#project-pagination-container a.page-link').forEach(link => {
        link.addEventListener('click', function (event) {
            event.preventDefault();
            const selectedPage = parseInt(this.getAttribute('data-page'));
            
            if (!isNaN(selectedPage)) {

                page_data_count = parseInt($('#project_page_count').val());
                searchText = $('#project_dt_search').val();
                pageNumber = selectedPage;

                loadProjects(pageNumber, page_data_count, searchText);
            }
        });
    });
}


function DeleteProject(ProjectId) {
    $.ajax({
        url: `/project/deleteproject?ProjectId=${ProjectId}`,
        type: 'GET',
        success: function () {
            FetchProject();
        }
    });
}