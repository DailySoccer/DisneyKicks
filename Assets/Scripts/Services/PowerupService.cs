using System;



public class PowerupService : IPowerupService, IDisposable
{
    public const int MAXPOWERUPSPORTERO = 4;
    public const int MAXPOWERUPSTIRADOR = 5;

    // power ups activos en este instante
    Powerup m_shooterPowerup;
    Powerup m_goalkeeperPowerup;

    public Powerup ShooterPowerup
    {
        get{ return m_shooterPowerup; }
        set{ m_shooterPowerup = value; usedShooterPowerup = true; }
    }

    public Powerup GoalkeeperPowerup
    {
        get{ return m_goalkeeperPowerup; }
        set{ m_goalkeeperPowerup = value; usedGoalkeeperPowerup = true; }
    }

    public bool usedShooterPowerup = false;
    public bool usedGoalkeeperPowerup = false;

    public static PowerupService instance;


    public static PowerupInventory ownInventory {
        set { m_ownInventory = value; }
        get {
            // asegurarse de que el ownInventory no pueda ser nulo
            if (m_ownInventory == null)
                m_ownInventory = new PowerupInventory();
            return m_ownInventory;
        } 
    }
    private static PowerupInventory m_ownInventory;
    public static PowerupInventory rivalInventory;

    public PowerupService() {
        instance = this;

        ServiceLocator.Register<IPowerupService>( this );

        ServiceLocator.Request<IShotResultService>().RegisterListener(Clean);

        if(rivalInventory == null) rivalInventory = new PowerupInventory(true);
    }

    private event Action<PowerupUsage> PowerupUsed = null;

    public void RegisterListener(Action<PowerupUsage> listener) {
        PowerupUsed += listener;
    }

    public void Clean(ShotResult _info)
    {
        usedGoalkeeperPowerup = false;
        usedShooterPowerup = false;
        ServiceLocator.Request<IGameplayService>().ResetTime();
    }

    public void UnregisterListener(Action<PowerupUsage> listener) {
        PowerupUsed -= listener;
    }

    public void OnPowerUpUsed(PowerupUsage _info) {
        if (PowerupUsed != null) {
            PowerupUsed( _info );
        }
    }

    public bool IsPowerActive(Powerup _powerup)
    {
        int id = (int)_powerup;
        if(id >= MAXPOWERUPSTIRADOR)
        {
            return usedGoalkeeperPowerup && (_powerup == GoalkeeperPowerup);
        }
        else
        {
            return usedShooterPowerup && (_powerup == ShooterPowerup);
        }
    }

    public void UsePowerup(Powerup _powerup)
    {
        int id = (int)_powerup;
        bool success = false;
        PowerupUsage info = new PowerupUsage();
        info.AbsId = id;
        info.Id = id; //si es de portero, se cambia mas tarde
        info.Own = false;
        info.Value = _powerup;
        bool isGoalkeeperPowerup = id >= MAXPOWERUPSTIRADOR;

        //proteccion para no usar 2 powerups al mismo tiempo
        if((!isGoalkeeperPowerup && PowerupService.instance.usedShooterPowerup) || (isGoalkeeperPowerup && PowerupService.instance.usedGoalkeeperPowerup))
        {
            return;
        }

        if(GameplayService.IsGoalkeeper())
        {
            if(id >= MAXPOWERUPSTIRADOR)
            {
                success = m_ownInventory.UsePowerup(id - MAXPOWERUPSTIRADOR, GameMode.GoalKeeper);
                info.Own = true;
            }
            else
            {
                success = rivalInventory.UsePowerup(id, GameMode.Shooter);
            }
        }
        else
        {
            if(id >= MAXPOWERUPSTIRADOR)
            {
                success = rivalInventory.UsePowerup(id - MAXPOWERUPSTIRADOR, GameMode.GoalKeeper);
            }
            else
            {
                success = m_ownInventory.UsePowerup(id, GameMode.Shooter);
                info.Own = true;
            }
        }

        if(success)
        {
            if(id >= MAXPOWERUPSTIRADOR)
            {
                GoalkeeperPowerup = _powerup;
                info.Mode = GameMode.GoalKeeper;
                info.Id = id - MAXPOWERUPSTIRADOR;
            }
            else
            {
                ShooterPowerup = _powerup;
                info.Mode = GameMode.Shooter;
            }
            //lanzar evento
            OnPowerUpUsed( info );
            PersistenciaManager.instance.SavePowerUps();
        }
    }

    public void Dispose() {
        PowerupUsed = null;
        ServiceLocator.Remove<IPowerupService>();
    }


}
