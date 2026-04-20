public interface ITriggerCheckeable
{
    bool isAggroed {get; set;}
    bool isInStrikingDistance {get; set;}

    void SetAggroStatus(bool newAggroStatus);
    void SetStrikingDistanceBool(bool newStrikingDistanceBool);
}
