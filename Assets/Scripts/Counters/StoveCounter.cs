using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static CuttingCounter;
using static UnityEngine.CullingGroup;

public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    public event EventHandler<onStateChangedEventArgs> OnStateChanged;
    public class onStateChangedEventArgs {
        public State state;
    }
    public enum State { 
      Idle,
      Frying,
      Fried,
      Burned
    }
    

    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;


    private State state;
    private float fryingTimer;
    private FryingRecipeSO fryingRecipeSO;
    private float burningTimer;
    private BurningRecipeSO burningRecipeSO;

    private void Start()
    {
        state = State.Idle;
    }
    private void Update()
    {
        if (HasKitchenObject())
        {
            switch (state)
            {
                case State.Idle:
                    break;
                case State.Frying:
                    fryingTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = fryingTimer / fryingRecipeSO.FryingTimerMax
                    });

                    if (fryingTimer > fryingRecipeSO.FryingTimerMax)
                    {
                        
                        
                        GetKitchenObject().DestroySelf();

                        KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);

                        state = State.Fried;
                        burningTimer = 0f;
                        burningRecipeSO = GetBurningRecipeSOWithOutput(GetKitchenObject().GetKitchenObjectSO());

                        OnStateChanged?.Invoke(this, new onStateChangedEventArgs {
                                state = state
                            });
                    }
                    break;
                case State.Fried:
                    burningTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = burningTimer / burningRecipeSO.BurningTimerMax
                    });

                    if (burningTimer > burningRecipeSO.BurningTimerMax)
                    {


                        GetKitchenObject().DestroySelf();

                        KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);

                        state = State.Burned;

                        OnStateChanged?.Invoke(this, new onStateChangedEventArgs
                        {
                            state = state
                        });

                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = 0f
                        });
                    }
                    break;
                case State.Burned:
                    break;

            }
        }
        

    }

    public override void Interact(Player player) {

        if (!HasKitchenObject())
        {
            //there is no kitchen object here 
            if (player.HasKitchenObject())
            {
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    player.GetKitchenObject().SetKitchenObjectParent(this);

                    fryingRecipeSO = GetFryingRecipeSOWithOutput(GetKitchenObject().GetKitchenObjectSO());
                    state = State.Frying;
                    fryingTimer = 0f;

                    OnStateChanged?.Invoke(this, new onStateChangedEventArgs
                    {
                        state = state
                    });

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = fryingTimer / fryingRecipeSO.FryingTimerMax
                    });

                }
            }
            else { 
            
            }

        }
        else
        {
            // there is a kitchen object here
            if (player.HasKitchenObject())
            {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {

                    if (plateKitchenObject.TryAddIngridient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();

                        state = State.Idle;

                        OnStateChanged?.Invoke(this, new onStateChangedEventArgs
                        {
                            state = state
                        });

                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = 0f
                        });
                    }
                }
            }
            else
            {

                GetKitchenObject().SetKitchenObjectParent(player);
                state = State.Idle;

                OnStateChanged?.Invoke(this, new onStateChangedEventArgs
                {
                    state = state
                });

                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = 0f
                });
            }

        }

    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {

        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithOutput(inputKitchenObjectSO);
        return fryingRecipeSO != null;

    }
    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {

        FryingRecipeSO outputFryingRecipeSO = GetFryingRecipeSOWithOutput(inputKitchenObjectSO);
        if (outputFryingRecipeSO != null)
        {
            return outputFryingRecipeSO.output;
        }
        else
        {
            return null;
        }


    }

    private FryingRecipeSO GetFryingRecipeSOWithOutput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray)
        {
            if (fryingRecipeSO.input == inputKitchenObjectSO)
            {
                return fryingRecipeSO;
            }
        }

        return null;

    }

    private BurningRecipeSO GetBurningRecipeSOWithOutput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOArray)
        {
            if (burningRecipeSO.input == inputKitchenObjectSO)
            {
                return burningRecipeSO;
            }
        }

        return null;

    }
}
