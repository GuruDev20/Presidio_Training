import { CommonModule } from "@angular/common";
import { Component, OnDestroy, OnInit } from "@angular/core";
import { LucideIconsModule } from "../../../utils/lucide-icons.module";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { Subscription } from "rxjs";
import { AdminService } from "../../../services/admin.service";

@Component({
    selector: 'app-manage-agent',
    templateUrl: './manage-agent.component.html',
    standalone: true,
    imports:[CommonModule,LucideIconsModule,ReactiveFormsModule] 
})

export class ManageAgentComponent implements OnInit,OnDestroy{

    isAddDrawerOpen = false;
    isListDrawerOpen = false;
    isUpdateDrawerOpen = false;
    addAgentForm:FormGroup;
    updateAgentForm:FormGroup;
    agents:any[] = [];
    selectedAgent:any = null;
    errorMessage={add:'',update:''};
    successMessage={add:'',update:''};
    private subscriptions:Subscription[] = [];

    constructor(private fb:FormBuilder,private adminService:AdminService){
        this.addAgentForm=this.fb.group({
            username:['',[Validators.required,Validators.minLength(3),Validators.maxLength(20)]],
            email:['',[Validators.required,Validators.email]],
            password:['',[Validators.required,Validators.minLength(6),Validators.maxLength(20)]],
        });

        this.updateAgentForm=this.fb.group({
            username:['',[Validators.required,Validators.minLength(3),Validators.maxLength(20)]],
        });
    }

    ngOnInit(): void {
        this.fetchAgents();
    }

    ngOnDestroy(): void{
        this.subscriptions.forEach(sub => sub.unsubscribe());
    }

    toggleDrawer(section:'add'|'list'|'update'){
        this.isAddDrawerOpen= section === 'add' ? !this.isAddDrawerOpen : false;
        this.isListDrawerOpen = section === 'list' ? !this.isListDrawerOpen : false;
        this.isUpdateDrawerOpen = section === 'update' ? !this.isUpdateDrawerOpen : false;
        this.errorMessage={ add: '', update: '' };
        this.successMessage = { add: '', update: '' };

        if(section==='add'){
            this.addAgentForm.reset();
        }
        else if(section==='update'){
            this.updateAgentForm.reset();
            this.selectedAgent = null;
            this.fetchAgents();
        }
        else if(section==='list'){
            this.fetchAgents();
        }
    }

    fetchAgents(){
        const sub = this.adminService.getAgentDetails().subscribe({
            next: (response) => {
                if(response.success){
                    this.agents = response.data.$values || [];
                }
                else{
                    console.error('Failed to fetch agents:', response.message);
                    this.agents = [];
                }
            },
            error: (error) => {
                console.error('Error fetching agents:', error);
            }
        });
        this.subscriptions.push(sub);
    }

    onSubmitAddAgent(){
        if(this.addAgentForm.invalid){
            this.errorMessage.add = 'Please fill all required fields correctly.';
            return;
        }
        const agentData={
            username: this.addAgentForm.value.username,
            email: this.addAgentForm.value.email,
            password: this.addAgentForm.value.password
        };
        const sub = this.adminService.createAgent(agentData).subscribe({
            next:(response)=>{
                if(response.success){
                    this.successMessage.add = 'Agent created successfully.Email sent to agent.';
                    this.errorMessage.add = '';
                    this.addAgentForm.reset();
                    this.toggleDrawer('add');
                    this.fetchAgents();
                }
                else{
                    this.errorMessage.add = response.message || 'Failed to create agent.';
                    this.successMessage.add = '';
                }
            },
            error:(error)=>{
                console.error('Error creating agent:', error);
                this.errorMessage.add = 'An error occurred while creating the agent.';
                this.successMessage.add = '';
            }
        });
        this.subscriptions.push(sub);
    }

    deleteAgent(agentId:string){
        if(!confirm('Are you sure you want to delete this agent?')){
            return;
        }
        const sub = this.adminService.deleteAgent(agentId).subscribe({
            next: (response) => {
                if(response.success){
                    this.successMessage.update = 'Agent deleted successfully.';
                    this.errorMessage.update = '';
                    this.fetchAgents();
                }
                else{
                    this.errorMessage.update = response.message || 'Failed to delete agent.';
                    this.successMessage.update = '';
                }
            },
            error: (error) => {
                console.error('Error deleting agent:', error);
                this.errorMessage.update = 'An error occurred while deleting the agent.';
                this.successMessage.update = '';
            }
        });
        this.subscriptions.push(sub);
    }

    selectAgentForUpdate(agent:any){
        this.selectedAgent = agent;
        this.updateAgentForm.patchValue({
            id: agent.id,
            username: agent.username,
        });
    }

    onSubmitUpdateAgent(){
        if(this.updateAgentForm.invalid){
            this.errorMessage.update = 'Please fill all required fields correctly.';
            return;
        }
        const agentData = {
            id: this.selectedAgent.id,
            username: this.updateAgentForm.value.username,
        };
        const sub = this.adminService.updateAgent(agentData).subscribe({
            next: (response) => {
                if(response.success){
                    this.successMessage.update = 'Agent updated successfully.';
                    this.errorMessage.update = '';
                    this.toggleDrawer('update');
                    this.fetchAgents();
                }
                else{
                    this.errorMessage.update = response.message || 'Failed to update agent.';
                    this.successMessage.update = '';
                }
            },
            error: (error) => {
                console.error('Error updating agent:', error);
                this.errorMessage.update = 'An error occurred while updating the agent.';
                this.successMessage.update = '';
            }
        });
        this.subscriptions.push(sub);
    }
}