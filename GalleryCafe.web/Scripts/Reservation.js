document.addEventListener('DOMContentLoaded', function() {
    const tableGrid = document.querySelector('.table-grid');
    const bookTableBtn = document.getElementById('bookTableBtn');
    
    let selectedTable = null;

    // Dummy data for booked tables (for demo)
    const bookedTables = [102, 105, 108];

    // Render tables
    for (let i = 101; i <= 120; i++) {
        const tableDiv = document.createElement('div');
        tableDiv.classList.add('table-icon');
        tableDiv.setAttribute('data-table', i);

        // Mark booked tables
        if (bookedTables.includes(i)) {
            tableDiv.classList.add('booked');
            tableDiv.innerHTML = `<i class="fas fa-chair"></i> ${i} - Booked`;
        } else {
            tableDiv.innerHTML = `<i class="fas fa-chair"></i> Table ${i}`;
        }

        tableGrid.appendChild(tableDiv);

        // Click event for available tables
        if (!tableDiv.classList.contains('booked')) {
            tableDiv.addEventListener('click', function() {
                // Reset previous selection
                document.querySelectorAll('.table-icon').forEach(icon => {
                    icon.style.transform = 'scale(1)';
                    icon.style.backgroundColor = '#28a745'; // Reset background color
                });

                // Highlight selected table
                this.style.transform = 'scale(1.2)';
                this.style.backgroundColor = 'yellow';
                selectedTable = this.getAttribute('data-table');
                bookTableBtn.style.display = 'inline-block';
            });
        }
    }

    // Book selected table
    bookTableBtn.addEventListener('click', function() {
        if (selectedTable) {
            // Simulate AJAX request for booking
            setTimeout(() => {
                Swal.fire({
                    title: 'Success!',
                    text: `Table ${selectedTable} has been successfully booked.`,
                    icon: 'success',
                    confirmButtonText: 'OK'
                });
            }, 1000);
        }
    });

    // 3D Scene Animation on mouse move
    document.querySelector('.table-scene').addEventListener('mousemove', function(e) {
        const xAxis = (window.innerWidth / 2 - e.pageX) / 25;
        const yAxis = (window.innerHeight / 2 - e.pageY) / 25;
        tableGrid.style.transform = `rotateY(${xAxis}deg) rotateX(${yAxis}deg)`;
    });
});
