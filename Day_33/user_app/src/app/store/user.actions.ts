import { User } from "../models/user.model";
import { createAction, props } from "@ngrx/store";

export const addUser=createAction('[User] Add',props<{user:User}>());
export const setUsers=createAction('[User] Set',props<{users:User[]}>());