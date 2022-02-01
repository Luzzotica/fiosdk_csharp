public class Authorization 
{
  string actor;
  string permission;

  Authorization(string actor, string permission="active") 
  {
    this.actor = actor;
    this.permission = permission;
  }
}