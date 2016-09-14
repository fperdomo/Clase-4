# Clase 4

## Temas

- Repository Pattern
- Unit of Work
- Moviendo todo a servicios

##Repository Pattern

Empecemos con una definición:

> Un repositorio es el intermediario entre las capas de dominio y de datos, actuando como una colección de objetos en memoria. Los objetos clientes construyen especificaciones de consultas de manera declarativa, y las envían al repositorio para completarlas. Pueden agregarse o quitarse objetos del repositorio, ya que son una simple colección de objetos, y el código encapsulado por el repositorio se encargará de ejecutar las operaciones correspondientes detrás de escenas.

El patrón es una abstracción. Su propósito es reducir la compleidad y hacer que el resto del código sea ignorante de la persistencia.

###Creando el repositorio

Para crear los repositorios, hay que seguir una regla simple:

> No agregar nada a la clase repositorio hasta el momento en que lo necesitas.

Creemos entonces el Repositorio para los usuarios. Los repositorios deberán implementar la interfaz IDisposable, ya que se encargarán de desechar el Context de Entity Framework.

```C#

public interface IUserRepository : IDisposable
{
    IEnumerable<User> GetUsers();
    User GetUserByID(int userId);
    void InsertUser(User user);
    void DeleteUser(int userID);
    void UpdateUser(User user);
    void Save();
}

```

```C#

public class UserRepository : IUserRepository
{
	private TresanaContext context;

	public UserRepository(TresanaContext context)
	{
		this.context = context;
	}

    public IEnumerable<User> GetUsers()
    {
    	this.context.Users.toList();
    }

    public User GetUserByID(int userId)
    {
    	context.Users.Find(userId);
    }

    public void InsertUser(User user)
    {
    	context.Users.Add(user);
    }

    public void DeleteUser(int userID)
    {
    	User user = context.Users.Find(userId);
    	context.Users.Remove(user);
    }

    public void UpdateUser(User user)
    {
    	context.Entry(user).State = EntityState.Modified;
    }

    public void Save()
    {
    	context.SaveChanges();
    }

    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
    	if(!disposed && disposing)
    	{
    		context.Dispose();
    	}
    	disposed = true;
    }

    public void Dispose()
    {
    	Dispose(true);
    	GC.SuppressFinalize(this);
    }

}

```

Pasemos entonces a usar el repositorio en el UsersController.

```C#

public class UsersController : ApiController
{
    private IUserRepository userRepository;

    public UserController()
    {
        userRepository = new UserRepository(new TresanaContext());
    }

    public UserController(IUserRepository repository)
    {
        userRepository = repository;
    }

        // GET: api/Users
    public IHttpActionResult GetUsers()
    {
        IEnumerable<User> users = userRepository.GetUsers();
        return Ok(users);
    }

        // GET: api/Users/5
    [ResponseType(typeof(User))]
    public IHttpActionResult GetUser(int id)
    {
        User user = userRepository.GetUserByID(id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

        // PUT: api/Users/5
    [ResponseType(typeof(void))]
    public IHttpActionResult PutUser(int id, User user)
    {
        if (!ModelState.IsValid) //Permite chequear que el request contenga el formato correcto
        {
            return BadRequest(ModelState);
        }

        if (id != user.Id)
        {
            return BadRequest();
        }

        try
        {
            userRepository.UpdateUser(user);
            userRepository.Save();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return StatusCode(HttpStatusCode.NoContent);
    }

        // POST: api/Users
    [ResponseType(typeof(User))]
    public IHttpActionResult PostUser(User user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        userRepository.InsertUser(user);
        userRepository.Save();

        return CreatedAtRoute("DefaultApi", new { id = user.Id }, user); //Retorna 202 con el id del usuario creado
    }

        // DELETE: api/Users/5
    [ResponseType(typeof(User))]
    public IHttpActionResult DeleteUser(int id)
    {
        if (!UserExists(id))
        {
            return NotFound();
        }
        User user = userRepository.GetUserByID(id);
        userRepository.DeleteUser(id);
        userRepository.Save();

        return Ok();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            db.Dispose();
        }
        base.Dispose(disposing);
    }

    private bool UserExists(int id)
    {
        return userRepository.Count(e => e.Id == id) > 0;
    }
}

```

###Repositorio Genérico y Unit of Work

Crear una clase Repositorio por cada entidad puede resultar en mucho código redundante, y en actualizaciones parciales. Por ejemplo, supongamos que debemos actualizar diferentes tipos como parte de la misma transacción. Si cada uno utiliza un contexto distinto, uno puede tener éxito pero el otro fallar. Para minimizar esta redundancia de código, utilizaremos un repositorio genérico (utilizando Generics). Por otro lado, para asegurar que todos los repositorios usen el mismo contexto, utilizaremos la clase UnitOfWork.

Generemos entonces el Repositorio genérico.

```C#
public interface IRepository<TEntity> where TEntity : class
{
	IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");
    TEntity GetByID(object id);
    void Delete(object id);
    void Delete(TEntity entityToDelete);
    void Update(TEntity entityToUpdate);

}


public class GenericRepository<TEntity> : IRepository<IEntity>
    {
        internal TresanaContext context;
        internal DbSet<TEntity> dbSet;

        public GenericRepository(TresanaContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual TEntity GetByID(object id)
        {
            return dbSet.Find(id);
        }

        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
        }
    }

 ```

Y ahora creemos la clase UnitOfWork

```C#

public interface IUnitOfWork : IDisposable
{
	IRepository<User> UserRepository;
    IRepository<Task> TaskRepository;
    void Save();
}


public class UnitOfWork : IUnitOfWork
{
    private TresanaContext context;
    private GenericRepository<User> userRepository;
    private GenericRepository<Task> taskRepository;


	public UnitOfWork()
	{
		context = new TresanaContext();
	}

	public UnitOfWork(TresanaContext tresanaContext)
	{
		context = tresanaContext;
	}

    public GenericRepository<User> UserRepository
    {
        get
        {

            if (this.userRepository == null)
            {
                this.userRepository = new GenericRepository<User>(context);
            }
            return userRepository;
        }
    }

    public GenericRepository<Task> TaskRepository
    {
        get
        {
            if (this.taskRepository == null)
            {
                this.taskRepository = new GenericRepository<Task>(context);
            }
            return taskRepository;
        }
    }

    public void Save()
    {
        context.SaveChanges();
    }

    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                context.Dispose();
            }
        }
        this.disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
```

Por último, cambiemos nuestro UserController para que utilice UnitOfWork.

```C#

public class UsersController : ApiController
{
    private IUnitOfWork unitOfWork;

    public UserController()
    {
        unitOfWork = new UnitOfWork();
    }

    public UserController(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

        // GET: api/Users
    public IHttpActionResult GetUsers()
    {
        IEnumerable<User> users = unitOfWork.UserRepository.Get();
        return Ok(users);
    }

    .
    .
    .
}


```

##Moviendo todo a servicios

En el ejercicio anterior, pudimos crear gran entidades en nuestra base de datos, y exponer una api que permite realizar acciones CRUD con nuestros controladores.
Sin embargo, la lógica de la aplicación quedó en los controladores, los cuales son los responsables de mantener los repositorios de datos.

Para evitar esto, moveremos la lógica a servicios.

###Creando los servicios.

Para comenzar, crearemos un servicio por cada controlador, en el proyecto Tresana.Web.Services. Para crearlo crearemos una interfaz I[Clase]Service y una clase [Clase]Service que implemente dicha interfaz.
Ejemplo con la clase User:

```C#

public interface IUserService
{
	User GetUserById(int userId);
	IEnumerable<User> GetAllUsers();
	int CreateUser(User user);
	bool UpdateUser(int userId, User user);
	bool DeleteUser(int userId);
}

```

```C#

public class UserService : IUserService
{
	
	private readonly IUnitOFWork unitOfWork;

	public UserService()
	{
		unitOfWork = new UnitOfWork();
	}

	public UserService(IUnitOfWork unitOfWork)
	{
		this.unitOfWork = unitOfWork;
	}

	public IEnumerable<User> GetAllUsers()
	{
		return unitOfWork.UserRepository.Get();
	}
	.
	.
	.

}

```

Y por último, agregamos el servicio a los controladores.