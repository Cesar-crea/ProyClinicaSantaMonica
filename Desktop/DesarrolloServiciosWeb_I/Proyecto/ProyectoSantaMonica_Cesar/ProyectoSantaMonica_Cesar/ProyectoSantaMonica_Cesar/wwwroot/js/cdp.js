document.addEventListener("DOMContentLoaded", () => {

    const modalCitas = document.getElementById('buscarCitaModal');

    if (modalCitas) {
        modalCitas.addEventListener('shown.bs.modal', () => {
            cargarCitas();
        });
    }

    document.getElementById("btnBuscarCitas")
        ?.addEventListener("click", buscarCitas);
});

// 🔍 Buscar
function buscarCitas() {
    const texto = document.getElementById('textoCita').value;

    fetch('/ComprobanteDePago/BuscarCitas?texto=' + encodeURIComponent(texto))
        .then(res => res.json())
        .then(data => actualizarTablaCitas(data))
        .catch(err => console.error(err));
}

// 🔄 Cargar
function cargarCitas() {
    fetch('/ComprobanteDePago/BuscarCitas')
        .then(res => res.json())
        .then(data => actualizarTablaCitas(data))
        .catch(err => console.error(err));
}

// 🧱 Tabla
function actualizarTablaCitas(data) {
    const tbody = document.querySelector("#tablaCitas tbody");
    tbody.innerHTML = "";

    if (!data || data.length === 0) {
        tbody.innerHTML = `<tr><td colspan="7" class="text-center">Sin resultados</td></tr>`;
        return;
    }

    data.forEach(c => {
        tbody.innerHTML += `
        <tr>
            <td>${c.id}</td>
            <td>${c.paciente}</td>
            <td>${c.fecha}</td>
            <td>${c.hora}</td>
            <td>${c.medico}</td>
            <td>${c.motivo ?? ''}</td>
            <td>
               <button class="btn btn-seleccionar"
    onclick='seleccionarCita(
        ${c.id},
        ${JSON.stringify(c.paciente)},
        ${JSON.stringify(c.fecha)},
        ${JSON.stringify(c.hora)},
        ${JSON.stringify(c.medico)},
        ${JSON.stringify(c.motivo ?? "")}
    )'>
    Seleccionar
</button>
            </td>
        </tr>`;
    });
}


