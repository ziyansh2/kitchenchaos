using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress {
	public enum State { 
		Idle,
		Frying,
		Fried,
		Burned
	}

	public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;


	public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
	public class OnStateChangedEventArgs : EventArgs {
		public State state;
	}

	[SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
	[SerializeField] private BurningRecipeSO[] burningRecipeSOArray;
	private State state;

	private float fryingTimer;
	private float buringTimer;
	private FryingRecipeSO fryingRecipeSO;
	private BurningRecipeSO burningRecipeSO;

	private void Start() {
		state = State.Idle;
	}

	private void Update() {
		if (HasKitchenObject()) {
			switch (state) {
				case State.Idle: break;
				case State.Frying:
					fryingTimer += Time.deltaTime;
					if (fryingTimer >= fryingRecipeSO.fryingTimerMax) {
						GetKitchenObject().DestroySelf();
						KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);

						UpdateState(State.Fried);
						buringTimer = 0;
						burningRecipeSO = GetBurningRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
					}
					OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs() { progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax });
					break;
				case State.Fried:
					buringTimer += Time.deltaTime;
					if (buringTimer >= burningRecipeSO.buringTimerMax) {
						GetKitchenObject().DestroySelf();
						KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);
						
						UpdateState(State.Burned);
						buringTimer = 0;
					}
					OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs() { progressNormalized = buringTimer / burningRecipeSO.buringTimerMax });
					break;
				case State.Burned: break;
			}
		}
	}

	private void UpdateState(State newState) {
		state = newState;
		OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
	}

	public override void Interact(Player player) {
		if (!HasKitchenObject()) {
			//There is no kitchen Object
			if (player.HasKitchenObject()) {
				//Player is carrying something
				if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) {
					//Drop it to the counter
					player.GetKitchenObject().SetKitchenObjectParent(this);
					fryingRecipeSO = GetFryingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
					UpdateState(State.Frying);
					fryingTimer = 0;
					OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs() { progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax });
				}
			}
		} else {
			//There is a kitchen Object
			if (!player.HasKitchenObject()) {
				//Player is not carrying anything
				GetKitchenObject().SetKitchenObjectParent(player);
				UpdateState(State.Idle);
				OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs() { progressNormalized = 0 });
			}
		}
	}

	private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObejectSO) {
		return GetFryingRecipeSOWithInput(inputKitchenObejectSO) != null;
	}

	private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObejectSO) {
		return GetFryingRecipeSOWithInput(inputKitchenObejectSO)?.output;
	}
	private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObejectSO) {
		foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray) {
			if (fryingRecipeSO.input == inputKitchenObejectSO) {
				return fryingRecipeSO;
			}
		}
		return null;
	}

	private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObejectSO) {
		foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOArray) {
			if (burningRecipeSO.input == inputKitchenObejectSO) {
				return burningRecipeSO;
			}
		}
		return null;
	}

}
