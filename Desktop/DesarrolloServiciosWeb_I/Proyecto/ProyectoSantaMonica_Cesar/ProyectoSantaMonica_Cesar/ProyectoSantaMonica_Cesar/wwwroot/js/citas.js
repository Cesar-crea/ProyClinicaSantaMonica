function abrirHorarios() {

    let medicoInput =
        document.getElementById("medicoId");

    let fechaInput =
        document.getElementById("fechaCita");

    if (!medicoInput) {

        console.error("No existe medicoId");

        Swal.fire(
            "Error",
            "No se encontró el médico",
            "error"
        );

        return;
    }

    if (!fechaInput) {

        console.error("No existe fechaCita");

        Swal.fire(
            "Error",
            "No se encontró la fecha",
            "error"
        );

        return;
    }

    let idMedico = medicoInput.value;
    let fecha = fechaInput.value;

    mostrarHorarios(idMedico, fecha);
}

function mostrarHorarios(idMedico, fecha) {

    if (!idMedico) {
        Swal.fire("Error", "Seleccione un médico primero", "error");
        return;
    }

    if (!fecha) {
        Swal.fire("Error", "Seleccione una fecha", "error");
        return;
    }

    fetch(`/Cita/HorariosOcupados?idMedico=${idMedico}&fecha=${fecha}`)
        .then(r => r.text())
        .then(html => {

            document.getElementById("modalHorariosContainer").innerHTML = html;

            let modal = new bootstrap.Modal(
                document.getElementById("modalHorarios")
            );

            modal.show();
        })
        .catch(error => {
            console.error(error);

            Swal.fire(
                "Error",
                "No se pudieron cargar los horarios",
                "error"
            );
        });
}

/*
function mostrarHorarios(idMedico, fecha) {

    if (!idMedico) {
        Swal.fire("Error", "Seleccione un médico primero", "error");
        return;
    }

    if (!fecha) {
        Swal.fire("Error", "Seleccione una fecha", "error");
        return;
    }

    fetch(`/Cita/HorariosOcupados?idMedico=${idMedico}&fecha=${fecha}`)
        .then(r => r.json())
        .then(data => {

            let html = `
            <div class="modal fade show" style="display:block">
                <div class="modal-dialog">
                    <div class="modal-content">

                        <div class="modal-header">
                            <h5 class="modal-title">
                                Horarios ocupados
                            </h5>

                            <button type="button"
                                    onclick="cerrarModal()"
                                    class="btn-close">
                            </button>
                        </div>

                        <div class="modal-body">
            `;

            if (data.length === 0) {

                html += `
                    <p class="text-success">
                        No hay horarios ocupados
                    </p>
                `;
            }
            else {

                html += `
        <h6 class="fw-bold mb-3">
            Horarios ocupados
        </h6>

        <div class="table-responsive">
            <table class="table table-bordered table-hover align-middle">
                
                <thead class="table-light">
                    <tr>
                        <th>Hora</th>
                        <th>Paciente</th>
                        <th>Motivo</th>
                    </tr>
                </thead>

                <tbody>
    `;

                data.forEach(h => {

                    let horaFormateada = h.hora.substring(0, 5);

                    let paciente =
                        `${h.nombres} ${h.apellidos}`;

                    html += `
            <tr>
                <td>${horaFormateada}</td>
                <td>${paciente}</td>
                <td>${h.motivo ?? ''}</td>
            </tr>
        `;
                });

                html += `
                </tbody>
            </table>
        </div>
    `;
            }

            html += `
                        </div>
                    </div>
                </div>
            </div>
            `;

            document.getElementById("modalHorariosContainer").innerHTML = html;
        });
}

*/

function cerrarModal() {
    document.getElementById("modalHorariosContainer").innerHTML = "";
}

/*

// ============================
// PACIENTE
// ============================
function seleccionarPaciente(id, nombre) {

    document.getElementById("pacienteId").value = id;
    document.getElementById("pacienteNombre").value = nombre;

    let modalElement = document.getElementById('modalPacientes');

    let modal = bootstrap.Modal.getOrCreateInstance(modalElement);
    modal.hide();
}


// ============================
// MÉDICO
// ============================
function seleccionarMedico(id, nombre) {

    document.getElementById("medicoId").value = id;
    document.getElementById("medicoNombre").value = nombre;

    let modalElement = document.getElementById('modalMedicos');

    let modal = bootstrap.Modal.getOrCreateInstance(modalElement);
    modal.hide();
}

*/ 



document.addEventListener("click", function (e) {

    // MÉDICO
    if (e.target.classList.contains("seleccionar-medico")) {
        let id = e.target.getAttribute("data-id");
        let nombre = e.target.getAttribute("data-nombre");

        document.getElementById("medicoId").value = id;
        document.getElementById("medicoNombre").value = nombre;

        let modal = bootstrap.Modal.getInstance(document.getElementById('modalMedicos'));
        modal.hide();
    }

    // PACIENTE
    if (e.target.classList.contains("seleccionar-paciente")) {
        let id = e.target.getAttribute("data-id");
        let nombre = e.target.getAttribute("data-nombre");

        document.getElementById("pacienteId").value = id;
        document.getElementById("pacienteNombre").value = nombre;

        let modal = bootstrap.Modal.getInstance(document.getElementById('modalPacientes'));
        modal.hide();
    }

});

document.addEventListener("hidden.bs.modal", function () {
    document.body.classList.remove("modal-open");

    let backdrops = document.querySelectorAll(".modal-backdrop");
    backdrops.forEach(b => b.remove());
});

console.log("citas.js cargado");