using UnityEngine;


public abstract class BaseGimmickPlayCompornent : MonoBehaviour
{
    public virtual void GimmicPlayTriggerEnter( BaseGimmickMoveCompornent hitObject ) { }
    public virtual void GimmicPlayTriggerExit( BaseGimmickMoveCompornent hitObject ) { }
    public virtual void GimmicPlayTriggerStay( BaseGimmickMoveCompornent hitObject ) { }
    public virtual void GimmicPlayCollisionEnter( BaseGimmickMoveCompornent hitObject ) { }
    public virtual void GimmicPlayCollisionExit( BaseGimmickMoveCompornent hitObject ) { }
    public virtual void GimmicPlayCollisionStay( BaseGimmickMoveCompornent hitObject ) { }
}
