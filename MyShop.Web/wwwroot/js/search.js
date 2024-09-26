
let timeout = null;

function submitSearchForm() {
    clearTimeout(timeout);

    timeout = setTimeout(function () {
        const searchInput = document.getElementById('searchInput');
        const searchString = searchInput.value;

        // إرسال طلب البحث باستخدام fetch
        fetch(`/Customer/Home/Search?searchString=${encodeURIComponent(searchString)}`)
            .then(response => response.text())
            .then(data => {
                document.getElementById('searchResults').innerHTML = data;
            })
            .catch(error => console.error('Error:', error));
    }, 300); // تأخير 300 مللي ثانية
}

document.addEventListener('DOMContentLoaded', function () {
    document.getElementById('searchInput').addEventListener('input', submitSearchForm);
});




//// JavaScript for automatic search submission
//function submitSearchForm() {
//    const searchInput = document.getElementById('searchInput');
//    const searchString = searchInput.value;

//    // إرسال طلب البحث باستخدام fetch أو XMLHttpRequest
//    fetch(`/Customer/Home/Search?searchString=${encodeURIComponent(searchString)}`)
//        .then(response => response.text())
//        .then(data => {
//            document.getElementById('searchResults').innerHTML = data;
//        });
//}


//let timeout = null;
//function submitSearchForm() {
//    const searchInput = document.getElementById('searchInput');
//    clearTimeout(timeout);
//    timeout = setTimeout(function () {
//        const searchString = searchInput.value;
//        if (searchString) {
//            fetch(`/Customer/Home/Search?searchString=${encodeURIComponent(searchString)}`)
//                .then(response => response.text())
//                .then(data => {
//                    document.getElementById('searchResults').innerHTML = data;
//                });
//        } else {
//            fetch(`/Customer/Home/Search`)
//                .then(response => response.text())
//                .then(data => {
//                    document.getElementById('searchResults').innerHTML = data;
//                });
//        }
//    }, 500); // Delay in milliseconds
//}