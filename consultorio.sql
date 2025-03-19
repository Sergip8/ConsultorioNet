
- -----------------------------------------------------
-- Schema consultorio
-- -----------------------------------------------------
DROP SCHEMA IF EXISTS `consultorio` ;

-- -----------------------------------------------------
-- Schema consultorio
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `consultorio` DEFAULT CHARACTER SET utf8mb4 ;
USE `consultorio` ;

-- -----------------------------------------------------
-- Table `consultorio`.`informacion_medica_paciente`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `consultorio`.`informacion_medica_paciente` ;

CREATE TABLE IF NOT EXISTS `consultorio`.`informacion_medica_paciente` (
    `id` INT NOT NULL AUTO_INCREMENT,
    `blood_type` VARCHAR(5) NOT NULL,
    `height` SMALLINT(3) NULL,
    `weight` SMALLINT(3) NULL,
    `diseasses` TEXT NULL,
    `allergies` TEXT NULL,
    PRIMARY KEY (`id`));


-- -----------------------------------------------------
-- Table `consultorio`.`medicamento`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `consultorio`.`medicamento` ;

CREATE TABLE IF NOT EXISTS `consultorio`.`medicamento` (
    `id` INT NOT NULL AUTO_INCREMENT,
    `medicament_name` VARCHAR(50) NOT NULL,
    `concentracion` VARCHAR(20) NOT NULL,
    `dosificacion` VARCHAR(20) NOT NULL,
    `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`id`));

-- -----------------------------------------------------
-- Table `consultorio`.`usuario`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `consultorio`.`users` ;

CREATE TABLE IF NOT EXISTS `consultorio`.`users` (
    `id` BIGINT(20) NOT NULL AUTO_INCREMENT,
    `email` VARCHAR(255) NOT NULL,
    `is_active` INT(11) NOT NULL,
    `password` VARCHAR(255) NOT NULL,
    `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` TIMESTAMP NULL ,
    PRIMARY KEY (`id`),
    UNIQUE INDEX `email_UNIQUE` (`email` ASC) VISIBLE);

-- -----------------------------------------------------
-- Table `consultorio`.`informacion_personal`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `consultorio`.`informacion_personal` ;

CREATE TABLE IF NOT EXISTS `consultorio`.`informacion_personal` (
    `id` BIGINT(20) NOT NULL AUTO_INCREMENT,
    `address` VARCHAR(100) NOT NULL,
    `birth_date` DATETIME NULL,
    `e_civil` VARCHAR(20) NOT NULL,
    `gender` VARCHAR(20) NOT NULL,
    PRIMARY KEY (`id`));

-- -----------------------------------------------------
-- Table `consultorio`.`pacientes`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `consultorio`.`pacientes` ;

CREATE TABLE IF NOT EXISTS `consultorio`.`pacientes` (
    `id` INT(11) NOT NULL AUTO_INCREMENT,
    `document_type` ENUM('DNI', 'Pasaporte', 'Cédula', 'TI') NOT NULL,
    `firstname` VARCHAR(50) NOT NULL,
    `identity_number` VARCHAR(30) NOT NULL,
    `lastname` VARCHAR(50) NOT NULL,
    `tel` VARCHAR(20) NOT NULL,
    `user_id` BIGINT(20) NOT NULL,
    `informacion_personal_id` BIGINT(20) NOT NULL,
    `informacion_medica_id` INT NOT NULL,
    UNIQUE INDEX `identity_number_UNIQUE` (`identity_number` ASC) VISIBLE,
    PRIMARY KEY (`id`),
    INDEX `fk_pacientes_users1_idx` (`user_id` ASC) VISIBLE,
    INDEX `fk_pacientes_informacion_personal1_idx` (`informacion_personal_id` ASC) VISIBLE,
    INDEX `fk_pacientes_informacion_medica_paciente1_idx` (`informacion_medica_id` ASC) VISIBLE,
    CONSTRAINT `fk_pacientes_users1`
    FOREIGN KEY (`user_id`)
    REFERENCES `consultorio`.`users` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
    CONSTRAINT `fk_pacientes_informacion_personal1`
    FOREIGN KEY (`informacion_personal_id`)
    REFERENCES `consultorio`.`informacion_personal` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
    CONSTRAINT `fk_pacientes_informacion_medica_paciente1`
    FOREIGN KEY (`informacion_medica_id`)
    REFERENCES `consultorio`.`informacion_medica_paciente` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

-- -----------------------------------------------------
-- Table `consultorio`.`centro_medico`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `consultorio`.`centro_medico` ;

CREATE TABLE IF NOT EXISTS `consultorio`.`centro_medico` (
	`id` INT NOT NULL AUTO_INCREMENT,
    `address` VARCHAR(120) NOT NULL,
    `city` VARCHAR(30) NOT NULL,
    `description` VARCHAR(255) NOT NULL,
    `name` VARCHAR(80) NOT NULL,
    `postal_code` VARCHAR(10) NOT NULL,
    `region` VARCHAR(10) NOT NULL,
    `tel` VARCHAR(30) NOT NULL,
    PRIMARY KEY (`id`));
 
-- -----------------------------------------------------
-- Table `consultorio`.`consultorios`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `consultorio`.`consultorios` ;

CREATE TABLE IF NOT EXISTS `consultorio`.`consultorios` (
    `id` INT(11) NOT NULL AUTO_INCREMENT,
    `description` TEXT NOT NULL,
    `is_active` TINYINT(1) NOT NULL,
    `number` VARCHAR(50) NOT NULL,
    `type` SMALLINT(6) NOT NULL,
    `medical_center_id` INT NOT NULL,
    PRIMARY KEY (`id`),
    CONSTRAINT `fk_medical_center_id1`
    FOREIGN KEY (`medical_center_id`)
    REFERENCES `consultorio`.`centro_medico` (`id`) 
    ON DELETE NO ACTION ON UPDATE NO ACTION 
);
-- -----------------------------------------------------
-- Table `consultorio`.`informacion_profesional`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `consultorio`.`informacion_profesional` ;

CREATE TABLE IF NOT EXISTS `consultorio`.`informacion_profesional` (
    `id` INT(11) NOT NULL AUTO_INCREMENT,
    `hire_date` DATETIME NULL,
    `professional_number` VARCHAR(20) NOT NULL,
    `work_shift` VARCHAR(4) NOT NULL,
    `specialization` SMALLINT(6) NOT NULL,
    `consultorios_id` INT(11) NOT NULL,
    PRIMARY KEY (`id`),
    INDEX `fk_informacion-profesional_consultorios1_idx` (`consultorios_id` ASC) VISIBLE,
    CONSTRAINT `fk_informacion-profesional_consultorios1`
    FOREIGN KEY (`consultorios_id`)
    REFERENCES `consultorio`.`consultorios` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);
    
CREATE TABLE IF NOT EXISTS `consultorio`.`especialidades_medicas` (
    `id` SMALLINT(6) NOT NULL AUTO_INCREMENT,
    `nombre` VARCHAR(100) NOT NULL UNIQUE,
    PRIMARY KEY (`id`)
);

-- -----------------------------------------------------
-- Table `consultorio`.`doctores`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `consultorio`.`doctores` ;

CREATE TABLE IF NOT EXISTS `consultorio`.`doctores` (
    `id` INT(11) NOT NULL AUTO_INCREMENT,
    `document_type` ENUM('DNI', 'Pasaporte', 'Cédula') NOT NULL,
    `firstname` VARCHAR(50) NOT NULL,
    `identity_number` VARCHAR(20) NOT NULL,
    `lastname` VARCHAR(50) NOT NULL,
    `tel` VARCHAR(25) NULL,
	`especializacion_id` SMALLINT(6) NOT NULL,
    `informacion_personal_id` BIGINT(20) NOT NULL,
    `informacion_profesional_id` INT(11) NOT NULL,
    `user_id` BIGINT(20) NOT NULL,
    UNIQUE INDEX `identity_number_UNIQUE` (`identity_number` ASC) VISIBLE,
    PRIMARY KEY (`id`),
    INDEX `fk_doctores_informacion_personal1_idx` (`informacion_personal_id` ASC) VISIBLE,
    INDEX `fk_doctores_informacion_profesional1_idx` (`informacion_profesional_id` ASC) VISIBLE,
    INDEX `fk_doctores_users1_idx` (`user_id` ASC) VISIBLE,
	INDEX `fk_especializacion_idx` (`especializacion_id` ASC) VISIBLE,
    CONSTRAINT `fk_doctores_informacion_personal1`
    FOREIGN KEY (`informacion_personal_id`)
    REFERENCES `consultorio`.`informacion_personal` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
	CONSTRAINT `fk_especializacion1`
    FOREIGN KEY (`especializacion_id`)
    REFERENCES `consultorio`.`especialidades_medicas` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
    CONSTRAINT `fk_doctores_informacion_profesional1`
    FOREIGN KEY (`informacion_profesional_id`)
    REFERENCES `consultorio`.`informacion_profesional` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
    CONSTRAINT `fk_doctores_users1`
    FOREIGN KEY (`user_id`)
    REFERENCES `consultorio`.`users` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

-- -----------------------------------------------------
-- Table `consultorio`.`citas`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `consultorio`.`citas` ;

CREATE TABLE IF NOT EXISTS `consultorio`.`citas` (
    `id` BIGINT(20) NOT NULL AUTO_INCREMENT,
    `slot` SMALLINT(2) NOT NULL,
    `appointment_start_time` DATETIME NOT NULL,
    `created_date` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `response` VARCHAR(255) NOT NULL,
    `state` ENUM('Cancelada', 'Agendada', 'Cédula') NOT NULL,
    `type` SMALLINT(6) NOT NULL,
    `doctor_id` INT(11) NOT NULL,
    `patient_id` INT(11) NOT NULL,
    PRIMARY KEY (`id`),
    INDEX `FKa0culq17omm7ln12kktrip4em` (`doctor_id` ASC) VISIBLE,
    INDEX `FK6bhsn5deahcw1vvmglohg89t9` (`patient_id` ASC) VISIBLE,
    CONSTRAINT `FK6bhsn5deahcw1vvmglohg89t9`
    FOREIGN KEY (`patient_id`)
    REFERENCES `consultorio`.`pacientes` (`id`),
    CONSTRAINT `FKa0culq17omm7ln12kktrip4em`
    FOREIGN KEY (`doctor_id`)
    REFERENCES `consultorio`.`doctores` (`id`));

-- -----------------------------------------------------
-- Table `consultorio`.`diagnostico`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `consultorio`.`diagnostico` ;

CREATE TABLE IF NOT EXISTS `consultorio`.`diagnostico` (
    `id` BIGINT(20) NOT NULL AUTO_INCREMENT,
    `description` TEXT NOT NULL,
    `citas_id` BIGINT(20) NOT NULL,
    `created_date` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`id`),
    INDEX `fk_tratamiento_citas1_idx` (`citas_id` ASC) VISIBLE,
    CONSTRAINT `fk_tratamiento_citas1`
    FOREIGN KEY (`citas_id`)
    REFERENCES `consultorio`.`citas` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);



-- -----------------------------------------------------
-- Table `consultorio`.`tratamiento_medicamentos`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `consultorio`.`diagnostico_medicamentos` ;

CREATE TABLE IF NOT EXISTS `consultorio`.`diagnostico_medicamentos` (
     `id` BIGINT NOT NULL AUTO_INCREMENT,
     `observaciones` TEXT NOT NULL,
     `cantidad` INT NOT NULL,
     `medicamento_id` INT NOT NULL,
     `tratamiento_id` BIGINT(20) NOT NULL,
    INDEX `fk_user_1_medicamento_idx` (`medicamento_id` ASC) VISIBLE,
    INDEX `fk_user_1_tratamiento1_idx` (`tratamiento_id` ASC) VISIBLE,
    PRIMARY KEY (`id`),
    CONSTRAINT `fk_user_1_medicamento`
    FOREIGN KEY (`medicamento_id`)
    REFERENCES `consultorio`.`medicamento` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
    CONSTRAINT `fk_user_1_tratamiento1`
    FOREIGN KEY (`tratamiento_id`)
    REFERENCES `consultorio`.`diagnostico` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);


-- -----------------------------------------------------
-- Table `consultorio`.`roles`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `consultorio`.`roles` ;

CREATE TABLE IF NOT EXISTS `consultorio`.`roles` (
      `id` BIGINT NOT NULL AUTO_INCREMENT,
      `rol` VARCHAR(20) NOT NULL,
      PRIMARY KEY (`id`));
   


-- -----------------------------------------------------
-- Table `consultorio`.`doctores_medical_appointment`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `consultorio`.`doctores_medical_appointment` ;

CREATE TABLE IF NOT EXISTS `consultorio`.`doctores_medical_appointment` (
    `doctor_id` INT(11) NOT NULL,
    `medical_appointments_id` BIGINT(20) NOT NULL,
    UNIQUE INDEX `UK_8ytsad13sjyup1xmxiyb6d11f` (`medical_appointments_id` ASC) VISIBLE,
    INDEX `FKque32dv71bs59dpui6jjl6nqc` (`doctor_id` ASC) VISIBLE,
    CONSTRAINT `FKovflpj2l03xh99sb5e9hq4kjy`
    FOREIGN KEY (`medical_appointments_id`)
    REFERENCES `consultorio`.`citas` (`id`),
    CONSTRAINT `FKque32dv71bs59dpui6jjl6nqc`
    FOREIGN KEY (`doctor_id`)
    REFERENCES `consultorio`.`doctores` (`id`));


-- -----------------------------------------------------
-- Table `consultorio`.`pacientes_medical_appointments`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `consultorio`.`pacientes_medical_appointments` ;

CREATE TABLE IF NOT EXISTS `consultorio`.`pacientes_medical_appointments` (
    `patient_id` INT(11) NOT NULL,
    `medical_appointments_id` BIGINT(20) NOT NULL,
    UNIQUE INDEX `UK_mm3xkggtp3loeod2q4pta3is0` (`medical_appointments_id` ASC) VISIBLE,
    INDEX `FKhjkrlstx540uc4xueo25o97pj` (`patient_id` ASC) VISIBLE,
    CONSTRAINT `FK232bgb67i713avtw6au50n0or`
    FOREIGN KEY (`medical_appointments_id`)
    REFERENCES `consultorio`.`citas` (`id`),
    CONSTRAINT `FKhjkrlstx540uc4xueo25o97pj`
    FOREIGN KEY (`patient_id`)
    REFERENCES `consultorio`.`pacientes` (`id`));



-- -----------------------------------------------------
-- Table `consultorio`.`refresh_token`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `consultorio`.`refresh_token` ;

CREATE TABLE IF NOT EXISTS `consultorio`.`refresh_token` (
     `id` BIGINT(20) NOT NULL AUTO_INCREMENT,
    `created_date` DATETIME NULL DEFAULT NULL,
    `token` VARCHAR(255) NULL DEFAULT NULL,
    PRIMARY KEY (`id`));


-- -----------------------------------------------------
-- Table `consultorio`.`verification_token`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `consultorio`.`verification_token` ;

CREATE TABLE IF NOT EXISTS `consultorio`.`verification_token` (
      `id` BIGINT(20) NOT NULL AUTO_INCREMENT,
    `expiry_date` DATETIME NULL DEFAULT NULL,
    `token` VARCHAR(255) NULL DEFAULT NULL,
    `user_id` BIGINT(20) NULL DEFAULT NULL,
    PRIMARY KEY (`id`),
    INDEX `FKisq7777w4ebjoj2x0n9soxswp` (`user_id` ASC) VISIBLE,
    CONSTRAINT `FKisq7777w4ebjoj2x0n9soxswp`
    FOREIGN KEY (`user_id`)
    REFERENCES `consultorio`.`users` (`id`));


-- -----------------------------------------------------
-- Table `consultorio`.`Empleado`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `consultorio`.`empleados` ;

CREATE TABLE IF NOT EXISTS `consultorio`.`empleados` (
    `id` INT(11) NOT NULL AUTO_INCREMENT,
    `document_type` ENUM('DNI', 'Pasaporte', 'Cedula') NOT NULL,
    `firstname` VARCHAR(50) NOT NULL,
    `identity_number` VARCHAR(30) NOT NULL,
    `lastname` VARCHAR(50) NOT NULL,
    `tel` VARCHAR(20) NOT NULL,
    `user_id` BIGINT(20) NOT NULL,
    `informacion_personal_id` BIGINT(20) NOT NULL,
    UNIQUE INDEX `identity_number_UNIQUE` (`identity_number` ASC) VISIBLE,
    PRIMARY KEY (`id`),
    INDEX `fk_pacientes_users1_idx` (`user_id` ASC) VISIBLE,
    INDEX `fk_pacientes_informacion_personal1_idx` (`informacion_personal_id` ASC) VISIBLE,
    CONSTRAINT `fk_pacientes_users10`
    FOREIGN KEY (`user_id`)
    REFERENCES `consultorio`.`users` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
    CONSTRAINT `fk_pacientes_informacion_personal10`
    FOREIGN KEY (`informacion_personal_id`)
    REFERENCES `consultorio`.`informacion_personal` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

-- -----------------------------------------------------
-- Table `consultorio`.`user_roles
-- -----------------------------------------------------

DROP TABLE IF EXISTS `consultorio`.`user_roles`;

CREATE TABLE IF NOT EXISTS `consultorio`.`user_roles` (
    `user_id`  BIGINT(20) NOT NULL,
    `role_id` BIGINT NOT NULL,
    PRIMARY KEY (`user_id`, `role_id`),
    CONSTRAINT `fk_user_1_role1` FOREIGN KEY (`user_id`) REFERENCES `consultorio`.`users`(`id`),
    CONSTRAINT `fk_role_1_user1` FOREIGN KEY (`role_id`) REFERENCES `consultorio`.`roles`(`id`)
);

DROP PROCEDURE IF EXISTS `consultorio`.`sp_create_user`

DELIMITER //

CREATE PROCEDURE `consultorio`.`sp_create_user`(
    IN p_email VARCHAR(255),
    IN p_password VARCHAR(255),
    IN p_role INT
)
BEGIN
    DECLARE user_id BIGINT;

    -- Insertar el usuario en la tabla users
    INSERT INTO `consultorio`.`users` (email, is_active, password, created_at)
    VALUES (p_email, 1, p_password, NOW());

    -- Obtener el ID del usuario recién insertado
    SET user_id = LAST_INSERT_ID();

    -- Asignar el rol de "Paciente" (ID = 1)
    INSERT INTO `consultorio`.`user_roles` (user_id, role_id)
    VALUES (user_id, p_role);
END //

DELIMITER ;

CREATE TABLE IF NOT EXISTS `consultorio`.`sessions` (
    `id` VARCHAR(255) PRIMARY KEY,
    `user_id` BIGINT(20) NOT NULL,
    `ip_address` VARCHAR(45) NULL,
    `user_agent` TEXT NULL,
    `payload` LONGTEXT NOT NULL,
    `last_activity` INT NOT NULL,
    INDEX idx_user_id (`user_id`),
    INDEX idx_last_activity (`last_activity`),
    CONSTRAINT fk_sessions_user FOREIGN KEY (`user_id`) REFERENCES `consultorio`.`users`(`id`) ON DELETE CASCADE
);

CREATE TABLE `consultorio`.`password_reset_tokens` (
   ` email` VARCHAR(255) PRIMARY KEY,
    `token` VARCHAR(255) NOT NULL,
    `created_at` TIMESTAMP NULL
);





INSERT INTO `consultorio`.`especialidades_medicas` (`nombre`) VALUES
('Medicina General'),
('Pediatría'),
('Ginecología y Obstetricia'),
('Cardiología'),
('Dermatología'),
('Oftalmología'),
('Otorrinolaringología'),
('Neurología'),
('Psiquiatría'),
('Traumatología y Ortopedia'),
('Urología'),
('Endocrinología'),
('Gastroenterología'),
('Neumología'),
('Reumatología'),
('Nefrología'),
('Oncología'),
('Cirugía General'),
('Medicina Interna'),
('Anestesiología');

INSERT INTO `consultorio`.`roles` (`rol`, `id`) VALUES
                                                            ('paciente', 1),
                                                            ('doctor', 2),
                                                            ('empleado', 3),
                                                            ('admin', 4);

INSERT INTO `consultorio`.`users` (`email`, `is_active`, `password`) VALUES
	('paciente1@example.com', 1, 'password123'),
	('paciente2@example.com', 1, 'password123'),
    ('paciente3@example.com', 1, 'password123'),
    ('paciente4@example.com', 1, 'password123'),
	( 'doctor1@example.com', 1, 'password123'),
	( 'doctor2@example.com', 1, 'password123'),
    ( 'doctor3@example.com', 1, 'password123'),
    ( 'doctor4@example.com', 1, 'password123'),
	('empleado1@example.com', 1, 'password123'),
    ('empleado2@example.com', 1, 'password123'),
    ('empleado3@example.com', 1, 'password123'),
    ('empleado4@example.com', 1, 'password123'),
	( 'admin@example.com', 1, 'password123');
                                                                                      
INSERT INTO `consultorio`.`user_roles` (`user_id`, `role_id`) VALUES
                                                                                                                     (1, 1),
                                                                                                                     (1, 2),
                                                                                                                     (2, 3),
                                                                                                                     (2, 4);

INSERT INTO `consultorio`.`informacion_personal` (`address`, `birth_date`, `e_civil`, `gender`) VALUES
                                                                                                    ('Calle Falsa 123', '1990-01-01', 'Soltero', 'Masculino'),
                                                                                                    ('Avenida Siempre Viva 456', '1985-05-15', 'Casado', 'Femenino'),
                                                                                                    ('Boulevard de los Sueños Rotos 789', '1978-11-22', 'Divorciado', 'Masculino'),
                                                                                                    ('Calle del Sol 321', '1995-03-30', 'Soltero', 'Femenino'),
																									('Calle Falsa 123', '1990-01-01', 'Soltero', 'Masculino'),
                                                                                                    ('Avenida Siempre Viva 456', '1985-05-15', 'Casado', 'Femenino'),
                                                                                                    ('Boulevard de los Sueños Rotos 789', '1978-11-22', 'Divorciado', 'Masculino'),
                                                                                                     ('Avenida Siempre Viva 456', '1985-05-15', 'Casado', 'Femenino'),
                                                                                                    ('Boulevard de los Sueños Rotos 789', '1978-11-22', 'Divorciado', 'Masculino'),
                                                                                                    ('Calle del Sol 321', '1995-03-30', 'Soltero', 'Femenino');

INSERT INTO `consultorio`.`informacion_medica_paciente` (`blood_type`, `height`, `weight`, `diseasses`, `allergies`) VALUES
                                                                                                                         ('O+', 175, 70, 'Ninguna', 'Polvo'),
                                                                                                                         ('A-', 160, 55, 'Diabetes', 'Penicilina'),
                                                                                                                         ('B+', 180, 80, 'Hipertensión', 'Mariscos'),
                                                                                                                         ('AB+', 165, 60, 'Asma', 'Ninguna');

INSERT INTO `consultorio`.`pacientes` (`document_type`, `firstname`, `identity_number`, `lastname`, `tel`, `user_id`, `informacion_personal_id`, `informacion_medica_id`) VALUES
                                                                                                                                                                                  ('DNI', 'Juan', '12345678', 'Pérez', '555-1234', 1, 1, 1),
                                                                                                                                                                                  ('Pasaporte', 'María', '87654321', 'Gómez', '555-5678', 2, 2, 2),
                                                                                                                                                                                  ('Cédula', 'Carlos', '11223344', 'López', '555-9101', 3, 3, 3),
                                                                                                                                                                                  ('TI', 'Ana', '44332211', 'Martínez', '555-1122', 4, 4, 4);

INSERT INTO `consultorio`.`centro_medico` (`address`, `city`, `description`, `name`, `postal_code`, `region`, `tel`) VALUES
                                                                                                                         ('Calle Principal 123', 'Ciudad de México', 'Centro médico general', 'Clínica Central', '12345', 'CDMX', '555-1234'),
                                                                                                                         ('Avenida Secundaria 456', 'Guadalajara', 'Centro especializado en cardiología', 'Clínica del Corazón', '67890', 'JAL', '555-5678');


INSERT INTO `consultorio`.`consultorios` (`description`, `is_active`, `number`, `type`, `medical_center_id`) VALUES
                                                                                            ('Consulta General', 1, '101', 1, 1),
                                                                                            ('Consulta Cardiología', 1, '102', 2, 1),
                                                                                            ('Consulta Dermatología', 1, '103', 3, 1),
                                                                                            ('Consulta Pediatría', 1, '104', 4, 1);

INSERT INTO `consultorio`.`informacion_profesional` (`hire_date`, `professional_number`, `work_shift`, `specialization`, `consultorios_id`) VALUES
                                                                                                                                                ('2020-01-15', '12345', 'AM', 1, 1),
                                                                                                                                                ('2018-05-20', '54321', 'PM', 2, 2),
                                                                                                                                                ('2019-11-10', '67890', 'AM', 3, 3),
                                                                                                                                                ('2021-03-25', '09876', 'PM', 4, 4);


INSERT INTO `consultorio`.`medicamento` (`medicament_name`, `concentracion`, `dosificacion`) VALUES
                                                                                                 ('Paracetamol', '500mg', 'Cada 8 horas'),
                                                                                                 ('Ibuprofeno', '400mg', 'Cada 12 horas'),
                                                                                                 ('Amoxicilina', '250mg', 'Cada 6 horas'),
                                                                                                 ('Loratadina', '10mg', 'Cada 24 horas');






INSERT INTO `consultorio`.`empleados` (`document_type`, `firstname`, `identity_number`, `lastname`, `tel`, `user_id`, `informacion_personal_id`) VALUES
                                                                                                                                                         ('DNI', 'Roberto', '55667788', 'Sánchez', '555-5566', 9, 9),
                                                                                                                                                         ('Pasaporte', 'Lucía', '99887766', 'Gutiérrez', '555-9988', 10, 10);


INSERT INTO `consultorio`.`doctores` (`document_type`, `firstname`, `identity_number`, `lastname`, `tel`, `informacion_personal_id`, `informacion_profesional_id`, `user_id`, `especializacion_id`) VALUES
                                                                                                                                                                                      ('DNI', 'Dr Luis', '33445566', 'García', '5553344', 5, 1, 5, 1),
                                                                                                                                                                                      ('Pasaporte', 'Dra Sofía', '77889900', 'Hernández', '5557788', 6, 2, 6, 2),
                                                                                                                                                                                      ('Pasaporte', 'Dr Pedro', '99001122', 'Ramírez', '5559900', 7, 3, 7, 3),
                                                                                                                                                                                      ('DNI', 'Dra Laura', '11223344', 'Fernández', '5551122', 8, 4, 8, 4);


INSERT INTO `consultorio`.`citas` (`slot`, `appointment_start_time`, `response`, `state`, `type`, `doctor_id`, `patient_id`) VALUES
                                                                                                                                 (30, '2023-10-15 10:00:00', 'Cita de la especialidad Consulta General con el doctor Dr. Luis en el día 2023-10-15 a la hora 10:00 de duración 30 minutos para el paciente Juan Pérez', 'Agendada', 1, 1, 1),
                                                                                                                                 (45, '2023-10-16 14:00:00', 'Cita de la especialidad Consulta Cardiología con el doctor Dra. Sofía en el día 2023-10-16 a la hora 14:00 de duración 45 minutos para el paciente María Gómez', 'Agendada', 2, 2, 2),
                                                                                                                                 (60, '2023-10-17 09:00:00', 'Cita de la especialidad Consulta Dermatología con el doctor Dr. Pedro en el día 2023-10-17 a la hora 09:00 de duración 60 minutos para el paciente Carlos López', 'Agendada', 3, 3, 3),
                                                                                                                                 (30, '2023-10-18 11:00:00', 'Cita de la especialidad Consulta Pediatría con el doctor Dra. Laura en el día 2023-10-18 a la hora 11:00 de duración 30 minutos para el paciente Ana Martínez', 'Agendada', 4, 4, 4);

INSERT INTO `consultorio`.`diagnostico` (`description`, `citas_id`) VALUES
                                                                        ('Resfriado común', 1),
                                                                        ('Hipertensión arterial', 2),
                                                                        ('Dermatitis atópica', 3),
                                                                        ('Control de niño sano', 4);

INSERT INTO `consultorio`.`diagnostico_medicamentos` (`observaciones`, `cantidad`, `medicamento_id`, `tratamiento_id`) VALUES
                                                                                                                           ('Tomar con alimentos', 10, 1, 1),
                                                                                                                           ('Evitar alcohol', 20, 2, 2),
                                                                                                                           ('Tomar con agua', 15, 3, 3),
                                                                                                                           ('Tomar antes de dormir', 30, 4, 4);

    INSERT INTO `consultorio`.`refresh_token` (`created_date`, `token`) VALUES
    (NOW(), 'token1'),
    (NOW(), 'token2');


    INSERT INTO `consultorio`.`verification_token` (`expiry_date`, `token`, `user_id`) VALUES
    ('2023-12-31 23:59:59', 'verification_token1', 1),
    ('2023-12-31 23:59:59', 'verification_token2', 2);



INSERT INTO `consultorio`.`doctores_medical_appointment` (`doctor_id`, `medical_appointments_id`) VALUES
                                                                                                      (1, 1),
                                                                                                      (2, 2),
                                                                                                      (3, 3),
                                                                                                      (4, 4);

INSERT INTO `consultorio`.`pacientes_medical_appointments` (`patient_id`, `medical_appointments_id`) VALUES
                                                                                                         (1, 1),
                                                                                                         (2, 2),
                                                                                                         (3, 3),
                                                                                                         (4, 4);


