#Clase 3

##Temas

- Seguimos con ApiControllers
- Pasando a servicios
- Comenzar patrón Repository

## ApiControllers

- ResponseType
- ModelState
- CreatedAtRoute
- Dispose

## Services

### Ejercicio

Ya poseen la los endpoints básicos para las operaciones CRUD de las entidades del producto.
Sin embargo, es necesario agregar ahora endpoints para las siguientes acciones:
- Agregar un usuario a un equipo
- Agregar un usuario a un proyecto
- Asignar un usuario a una tarea

Realice la lógica necesaria, para que estas acciones puedan realizarse siguiendo los patrones de la arquitectura REST.

Para hacerlo, y evitar que la lógica de la aplicación sea mantenida en los mismos controllers, se recomienda abstraer dicha lógica a Servicios.
Los servicios son clases cuya responsabilidad radica en realizar estas transacciones de manera atómica, y deben programarse de tal manera que su ciclo de vida sea igual al de los controladores. Esto quiere decir, que deben implementar la interfaz IDisposable.
También es recomendable separar una interfaz de la implementación de los servicios, para mantener claros los contratos y facilitar el testing.
