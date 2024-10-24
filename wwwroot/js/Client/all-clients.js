document.addEventListener('DOMContentLoaded', function () {
    
    const pageSize = 5;
    let currentPage = 1;

    loadClients(currentPage, pageSize);

    function loadClients(pageNo, count) {
        $.ajax({
            url: `/client/allclients?pageNo=${pageNo}&count=${count}`,
            type: 'GET',
            success: function (response) {
                

                renderClients(response.Clients);
                renderPagination(response.TotalNumberOfPage, response.CurrentPageNumber, response.PreviousPageNumber, response.NextPageNumber);
            },
            error: function (error) {
                console.error('Error fetching clients:', error);
            }
        });
    }

    function renderClients(clients) {
        
        const clientContainer = document.getElementById('client-list-container');
        clientContainer.innerHTML = ''; 

        clients.forEach(client => {
            clientContainer.innerHTML += `
               <tr class="odd gradex">
				    <td class="center dt-type-numeric">${client.ClientId}</td>
				    <td class="center">${client.Name}</td>
				    <td class="center">${client.Email}</td>
				    <td class="center">${client.CountryCode}</td>
				    <td class="center">${client.PhoneNumber}</td>
				    <td class="center">${client.Address}</td>
				    <td class="center">${client.Description}</td>
				    <td class="center">
				    <a href="/Client/EditClientView?ClientId=${client.ClientId}" class="btn btn-tbl-edit">
				        <i class="material-icons">create</i>
				    </a>
				    <a href="/Client/DeleteClient?ClientId=${client.ClientId}" class="btn btn-tbl-delete">
				        <i class="material-icons">delete_forever</i>
				    </a>
				    </td>
               </tr>
            `;
           
        });
    }

    function renderPagination(totalPages, currentPage, PreviousPageNumber, NextPageNumber) {
        


        const paginationContainer = document.getElementById('pagination-container');
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

        document.querySelectorAll('#pagination-container a.page-link').forEach(link => {
            link.addEventListener('click', function (event) {
                event.preventDefault();
                const selectedPage = parseInt(this.getAttribute('data-page'));
                if (!isNaN(selectedPage)) {
                    loadClients(selectedPage, pageSize);
                }
            });
        });
    }
});
