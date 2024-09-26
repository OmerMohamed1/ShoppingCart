document.addEventListener('DOMContentLoaded', function () {
    function setupPaginationLinks() {
        const paginationLinks = document.querySelectorAll('.pagination a');
        paginationLinks.forEach(link => {
            link.addEventListener('click', function (event) {
                event.preventDefault();
                const url = this.href;
                fetch(url)
                    .then(response => response.text())
                    .then(data => {
                        const parser = new DOMParser();
                        const doc = parser.parseFromString(data, 'text/html');
                        const newContent = doc.getElementById('product-list').innerHTML;
                        document.getElementById('product-list').innerHTML = newContent;
                        window.history.pushState({ path: url }, '', url);
                        setupPaginationLinks(); // Re-setup the links after updating content
                    })
                    .catch(error => console.error('Error:', error));
            });
        });
    }

    setupPaginationLinks();
});
