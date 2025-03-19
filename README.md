# ConsultorioNet

ConsultorioNet is a web-based application designed to manage and streamline operations for medical or dental clinics. It provides tools for scheduling appointments, managing patient records, and tracking clinic activities.
Under construction
## Features

- **Appointment Scheduling**: Easily book, reschedule, or cancel appointments.
- **Patient Management**: Store and manage patient information securely.
- **Doctor Management**: Store and manage doctor information securely.

- **Reports and Analytics**: Generate reports to track clinic performance.
- **User Roles**: Support for admin, doctors, and staff roles with specific permissions.

## Running the Azure Function Project

To run the Azure Function project locally, follow these steps:

1. **Install Prerequisites**:
    - Install [Azure Functions Core Tools](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local).
    - Install the [.NET SDK](https://dotnet.microsoft.com/download).

2. **Clone the Repository**:
    ```bash
    git clone https://github.com/your-repo/ConsultorioNet.git
    cd ConsultorioNet
    ```

3. **Navigate to the Azure Function Project**:
    ```bash
    cd path/to/azure-function-project
    ```

4. **Restore Dependencies**:
    ```bash
    dotnet restore
    ```

5. **Run the Azure Function Locally**:
    ```bash
    func start
    ```

6. **Test the Function**:
    - Use tools like [Postman](https://www.postman.com/) or [cURL](https://curl.se/) to test the endpoints.
    - Ensure the local URL (e.g., `http://localhost:7071`) is used.

7. **Deploy to Azure** (Optional):
    - Follow the [Azure Functions deployment guide](https://learn.microsoft.com/en-us/azure/azure-functions/functions-deployment-technologies) to deploy your function to Azure.


## Technologies Used

- **Frontend**: Angular
- **Backend**: Azure Functions .Net 9
- **Database**: Mysql
- **Authentication**: JWT

## Contributing


## License

This project is licensed under the [MIT License](LICENSE).

## Contact

For questions or support, please contact [your-email@example.com].