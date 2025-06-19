import { createReducer,on} from "@ngrx/store";
import { User } from "../models/user.model";
import { addUser, setUsers } from "./user.actions";

export const initialState: User[] = [];

export const userReducer=createReducer(
    initialState,
    on(setUsers,(_,{ users }) => users),
    on(addUser, (state, { user }) => [...state, user])
)